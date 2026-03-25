## 언어 및 소통 설정
- 모든 답변과 코드 설명은 **한국어**로 작성해줘.

## 코딩 컨벤션 가이드 (C# / Unity)

이 프로젝트의 비서로서 다음의 코딩 규칙을 엄격히 준수하세요.

### 1. 명명 규칙
- **클래스/메서드/인터페이스:** `PascalCase`
- **프라이빗,퍼블릭 필드:** `camelCase` (언더바 없음)
- **메서드 매개변수:** `_camelCase` (언더바 접두어 필수)

### 2. 클래스 구조 순서
1. 필드 선언 (주석으로 //외부 의존성, //내부 의존성 구분)
2. 퍼블릭 초기화 및 제어 메서드 (Initialize, Setup, Release 등)
4. **Godot 가상 함수:** `_Ready`, `_Process`, `_PhysicsProcess` 순으로 배치 (최하단)

### 3. 설계 패턴 및 참조
- **수동 의존성 주입:** `Initialize(_dependency)` 패턴을 통해 노드 간 결합도를 낮춘다.
- **노드 참조 최적화:** `GetNode<T>()` 호출은 최소화하고, `_Ready`에서 캐싱하거나 `[Export]`를 통해 인스펙터에서 할당한다.
- **명시성:** `private`, `public`, `protected` 등 접근 제어자를 생략하지 않고 반드시 작성한다.

## 🛠 메모리 및 성능 최적화 원칙 (Unity C#)

우리 프로젝트는 60FPS 유지를 위해 GC(Garbage Collection) 발생과 메모리 단편화를 엄격히 제한한다. 모든 코드 생성 및 수정 시 다음 규칙을 반드시 준수하라.

### 1. 스택 메모리 활용 극대화 (Stack-First Memory Management)
- **구조체 참조 전달 (`in`, `ref`):** `Vector3`, `Transform3D`, `Color` 등 크기가 큰 Godot 구조체를 메서드 인자로 전달할 때 복사 비용을 줄이기 위해 `in` (읽기 전용 참조) 또는 `ref` 키워드를 사용하라.
- **값 형식(Struct) 우선:** 데이터 중심의 작은 객체는 `class` 대신 `struct`로 선언하여 스택 메모리를 활용하라. (단, 16바이트를 초과하거나 잦은 복사가 발생하는 경우는 `ref` 전달을 강제하라)
- **`stackalloc` 및 `Span<T>`:** 함수 내부의 단기 임시 버퍼(레이캐스트 결과, 정점 계산 등)는 힙 대신 `stackalloc`을 사용하여 스택에 할당하라. 
- **로컬 함수 및 정적 메서드:** 힙 할당을 유발하는 델리게이트 캡처를 피하기 위해, 가능한 경우 `static` 로컬 함수를 정의하여 사용하라.

### 2. 메모리 단편화 및 할당 방지 (Minimize Fragmentation)
- **컬렉션 초기 용량 설정:** `List`, `Dictionary` 등을 생성할 때 예상되는 최대 크기(`Capacity`)를 생성자 인자로 넘겨 내부 배열의 잦은 재할당과 복사를 방지하라.
- **Boxing/Unboxing 제거:** 제네릭을 적극 활용하여 값 형식이 `object`나 `Variant`로 변환되는 과정을 차단하라.
  - **Interface 박싱 주의:** 구조체를 인터페이스로 다룰 때 발생하는 박싱을 방지하기 위해 제네릭 제약 조건(`where T : struct, IInterface`)을 사용하라.
  - **Enum Dictionary:** `Enum`을 키로 사용하는 딕셔너리는 박싱이 발생할 수 있으므로 주의하거나 전용 비교자를 사용하라.
- **고정 크기 버퍼 활용:** 자주 사용하는 대규모 임시 버퍼는 `System.Buffers.ArrayPool<T>`을 사용하여 힙 할당 없이 재사용하라.

### 3. Godot 전용 최적화
- **`StringName` 활용:** 애니메이션 이름, 노드 경로, 시그널 이름 등 빈번히 참조되는 문자열은 `StringName` 타입으로 선언하고 `static readonly` 필드에 캐싱하여 사용하라.
- **캐싱(Caching):** `GetNode<T>()`, `GetTree()`, `ResourceLoader.Load()` 등은 `_Ready`에서 변수에 캐싱하고 `_Process` 내 반복 호출을 금지한다.
- **Physics API 주의:** 레이캐스트 등 물리 쿼리 시 매번 새로운 `Dictionary`를 생성하는 API 대신, 가능한 경우 할당이 적은 방식을 택하거나 쿼리 횟수를 최적화하라.
- **Typed Array & Dictionary:** Godot 엔진 API와 통신할 때는 `Godot.Collections.Array<T>`와 같이 타입을 명시한 컬렉션을 사용하여 런타임 타입 체크 비용을 줄여라.

### 4. GC 발생 최소화 (Minimize GC Alloc)
- **LINQ 사용 금지:** `Where`, `Select`, `ToList` 등 모든 LINQ 메서드는 힙 할당을 유발하므로 사용하지 않는다. 대신 `for` 또는 `foreach` 루프를 사용하라.
- **문자열 연산 주의:** 루프 내에서 문자열 더하기(`+`)를 금지한다. 빈번한 문자열 조립이 필요하면 `StringBuilder`를 사용하라.
- **초기화 시 할당:** `new` 키워드를 통한 객체 생성은 `_Ready`나 초기화 시점에 수행하고, 런타임(Process) 중에는 기존 객체를 재사용(Pooling)하라.

### 5. 데이터 레이아웃 및 성능 안정성 (Advanced)
- **메모리 패딩 최적화:** `struct` 선언 시 데이터 필드를 크기순으로 정렬하여 메모리 단편화를 방지하라.
- **Typed 컬렉션 강제:** Godot API와의 상호작용 시 `Godot.Collections.Array<T>`와 `Godot.Collections.Dictionary<T, V>`를 사용하여 엔진 내부 마샬링 시 발생하는 런타임 캐스팅 오버헤드를 줄여라.
- **Task 보다는 Signal/ValueTask:** 비동기 작업 시 불필요한 `Task` 객체 생성을 지양하고, Godot 내장 시그널 대기나 `ValueTask`를 검토하라.
- **수동 메모리 해제:** `Node`나 `RefCounted`가 아닌 `GodotObject` 파생 클래스는 사용 후 반드시 `Dispose()` 또는 `Free()`를 호출하여 엔진 측 메모리 누수를 차단하라.