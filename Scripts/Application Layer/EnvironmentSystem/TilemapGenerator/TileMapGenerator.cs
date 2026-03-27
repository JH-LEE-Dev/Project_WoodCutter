using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class TileMapGenerator : Node
{
    public event Action<List<Vector3>> TilemapGeneratedEvent;

    [ExportGroup("설정")]
    [Export] private int width = 150;
    [Export] private int height = 150;
    [Export] private float scale = 25f;
    [Export] private int seed;

    [ExportGroup("섬 방지 및 Falloff 설정")]
    [Export] private bool useIslandPrevention = true;
    [Export] private float falloffA = 3f;
    [Export] private float falloffB = 6.5f;
    [Export] private float waterThreshold = 0.38f;

    [ExportGroup("타일 레이어 및 셋업")]
    [Export] private TileMapLayer groundLayer;
    [Export] private TileMapLayer collisionLayer;

    [ExportSubgroup("타일 소스 ID (각 PNG 파일의 Source ID)")]
    [Export] private int waterSourceId = 0;
    [Export] private int sandSourceId = 1;
    [Export] private int grassSourceId = 2;
    [Export] private int mountainSourceId = 3;

    // // 내부 데이터 및 캐싱
    private float[] noiseValues;
    private bool[] visited;
    private bool[] isShoreline;
    private FastNoiseLite noise;
    private float halfCellY;

    // // 재사용 컬렉션 (GC 최소화)
    private List<int> largestBlob;
    private List<int> currentBlob;
    private List<int> shorelineList;
    private List<int> innerEdgesList;
    private List<Vector3> grassPositions;
    private List<Vector3> walkablePositions;
    private Dictionary<Vector2I, int> positionToIndex;
    private Queue<int> bfsQueue;

    private int playerIdx = -1;
    private int portalIdx = -1;

    // // 퍼블릭 초기화 및 제어 메서드

    public void InitializeMapData()
    {
        int size = width * height;
        noiseValues = new float[size];
        visited = new bool[size];
        isShoreline = new bool[size];

        largestBlob = new List<int>(size);
        currentBlob = new List<int>(size);
        shorelineList = new List<int>(size / 4);
        innerEdgesList = new List<int>(size / 4);
        grassPositions = new List<Vector3>(size / 2);
        walkablePositions = new List<Vector3>(size);
        positionToIndex = new Dictionary<Vector2I, int>(size);
        bfsQueue = new Queue<int>(size);

        if (seed == 0) seed = (int)GD.Randi();

        noise = new FastNoiseLite();
        noise.Seed = seed;
        noise.Frequency = 1.0f / scale;
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;

        if (groundLayer != null)
        {
            halfCellY = groundLayer.TileSet.TileSize.Y * 0.5f;
        }
    }

    public void GenerateMap(IBootstrapProvider _bootstrapProvider)
    {
        if (groundLayer == null || collisionLayer == null) return;

        groundLayer.GetParent().RemoveChild(groundLayer);
        collisionLayer.GetParent().RemoveChild(collisionLayer);
        _bootstrapProvider.GetTargetSceneNode().AddChild(groundLayer);
        _bootstrapProvider.GetTargetSceneNode().AddChild(collisionLayer);

        groundLayer.Clear();
        collisionLayer.Clear();

        GenerateNoiseMap();
        RemoveIslands();
        DetermineSpawns();
        ApplyTiles();

        TilemapGeneratedEvent?.Invoke(grassPositions);
        GD.Print("AAAA");
    }

    public Vector3 GetPlayerSpawnPosition() => GetWorldPos(playerIdx);
    public Vector3 GetPortalSpawnPosition() => GetWorldPos(portalIdx);
    public List<Vector3> GetGrassTileWorldPositions() => grassPositions;
    public List<Vector3> GetWalkableTileWorldPositions() => walkablePositions;

    public bool IsWalkable(Vector2I _cellPos) => positionToIndex.ContainsKey(_cellPos);

    public Vector2I WorldToCell(Vector3 _worldPos)
    {
        if (groundLayer == null) return Vector2I.Zero;
        Vector2 pos2D = new Vector2(_worldPos.X, _worldPos.Y - halfCellY);
        return groundLayer.LocalToMap(pos2D);
    }

    public Vector3 CellToWorld(Vector2I _cellPos)
    {
        if (groundLayer == null) return Vector3.Zero;
        Vector2 pos2D = groundLayer.MapToLocal(_cellPos);
        return new Vector3(pos2D.X, pos2D.Y + halfCellY, 0);
    }

    // // Godot 가상 함수

    public override void _Ready()
    {

    }

    // // 프라이빗 로직 메서드

    private void GenerateNoiseMap()
    {
        float invWidth = 1f / width;
        float invHeight = 1f / height;

        for (int y = 0; y < height; y++)
        {
            int rowOffset = y * width;
            float ny = y * (y - 1f != 0 ? 1f / (height - 1f) : 1f) * 2f - 1f;

            for (int x = 0; x < width; x++)
            {
                float val = (noise.GetNoise2D(x, y) + 1.0f) * 0.5f;

                if (useIslandPrevention)
                {
                    float nx = x * (x - 1f != 0 ? 1f / (width - 1f) : 1f) * 2f - 1f;
                    float dist = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
                    val -= EvaluateFalloff(dist);
                }

                noiseValues[x + rowOffset] = val;
            }
        }
    }

    private void RemoveIslands()
    {
        int size = width * height;
        Array.Clear(visited, 0, size);
        largestBlob.Clear();

        ReadOnlySpan<int> dx = stackalloc int[] { 1, -1, 0, 0 };
        ReadOnlySpan<int> dy = stackalloc int[] { 0, 0, 1, -1 };

        for (int i = 0; i < size; i++)
        {
            if (noiseValues[i] < waterThreshold || visited[i]) continue;

            currentBlob.Clear();
            bfsQueue.Clear();

            bfsQueue.Enqueue(i);
            visited[i] = true;

            while (bfsQueue.Count > 0)
            {
                int c = bfsQueue.Dequeue();
                currentBlob.Add(c);

                int cx = c % width;
                int cy = c / width;

                for (int j = 0; j < 4; j++)
                {
                    int nx = cx + dx[j];
                    int ny = cy + dy[j];

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        int ni = nx + ny * width;
                        if (!visited[ni] && noiseValues[ni] >= waterThreshold)
                        {
                            visited[ni] = true;
                            bfsQueue.Enqueue(ni);
                        }
                    }
                }
            }

            if (currentBlob.Count > largestBlob.Count)
            {
                largestBlob.Clear();
                largestBlob.AddRange(currentBlob);
            }
        }

        Array.Clear(visited, 0, size);
        for (int i = 0; i < largestBlob.Count; i++)
        {
            visited[largestBlob[i]] = true;
        }

        for (int i = 0; i < size; i++)
        {
            if (noiseValues[i] >= waterThreshold && !visited[i])
            {
                noiseValues[i] = waterThreshold - 0.05f;
            }
        }
    }

    private void DetermineSpawns()
    {
        int size = width * height;
        Array.Clear(isShoreline, 0, size);
        shorelineList.Clear();
        innerEdgesList.Clear();

        for (int i = 0; i < largestBlob.Count; i++)
        {
            int idx = largestBlob[i];
            int x = idx % width;
            int y = idx / width;

            if (IsWater(x + 1, y) || IsWater(x - 1, y) || IsWater(x, y + 1) || IsWater(x, y - 1))
            {
                isShoreline[idx] = true;
                shorelineList.Add(idx);
            }
        }

        ReadOnlySpan<int> dx = stackalloc int[] { 1, -1, 0, 0 };
        ReadOnlySpan<int> dy = stackalloc int[] { 0, 0, 1, -1 };

        for (int i = 0; i < largestBlob.Count; i++)
        {
            int idx = largestBlob[i];
            if (isShoreline[idx]) continue;

            int x = idx % width;
            int y = idx / width;

            for (int j = 0; j < 4; j++)
            {
                int nx = x + dx[j];
                int ny = y + dy[j];

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (isShoreline[nx + ny * width])
                    {
                        innerEdgesList.Add(idx);
                        break;
                    }
                }
            }
        }

        Random rand = new Random();
        if (innerEdgesList.Count > 0)
            portalIdx = innerEdgesList[rand.Next(innerEdgesList.Count)];
        else if (shorelineList.Count > 0)
            portalIdx = shorelineList[rand.Next(shorelineList.Count)];
        else
            portalIdx = largestBlob.Count > 0 ? largestBlob[0] : -1;

        playerIdx = portalIdx;
    }

    private void ApplyTiles()
    {
        int size = width * height;
        grassPositions.Clear();
        walkablePositions.Clear();
        positionToIndex.Clear();

        float sandT = waterThreshold + 0.1f;
        float mountT = 0.7f;
        Vector3 portalPos = GetPortalSpawnPosition();

        Vector2I atlasCoords = Vector2I.Zero;

        for (int i = 0; i < size; i++)
        {
            float v = noiseValues[i];
            Vector2I cellPos = new Vector2I(i % width, i / width);

            if (v < waterThreshold)
            {
                // 물 타일 배치
                collisionLayer.SetCell(cellPos, waterSourceId, atlasCoords);
            }
            else
            {
                Vector3 pos = GetWorldPos(i);
                positionToIndex[cellPos] = walkablePositions.Count;
                walkablePositions.Add(pos);

                int currentSourceId;
                if (v < sandT)
                    currentSourceId = sandSourceId;
                else if (v < mountT)
                {
                    currentSourceId = grassSourceId;
                    // 포탈 주변에는 풀 장식( grassPositions) 생성 방지
                    if ((pos - portalPos).LengthSquared() > 2.25f)
                    {
                        grassPositions.Add(pos);
                    }
                }
                else
                    currentSourceId = mountainSourceId;

                // 지면 타일 배치
                groundLayer.SetCell(cellPos, currentSourceId, atlasCoords);
            }
        }
    }

    private bool IsWater(int _x, int _y)
    {
        if (_x < 0 || _x >= width || _y < 0 || _y >= height) return true;
        return noiseValues[_x + _y * width] < waterThreshold;
    }

    private Vector3 GetWorldPos(int _idx)
    {
        if (_idx < 0) return Vector3.Zero;
        return CellToWorld(new Vector2I(_idx % width, _idx / width));
    }

    private float EvaluateFalloff(float _v)
    {
        float p = Mathf.Pow(_v, falloffA);
        float q = Mathf.Pow(falloffB - (falloffB * _v), falloffA);
        return p / (p + q);
    }
}
