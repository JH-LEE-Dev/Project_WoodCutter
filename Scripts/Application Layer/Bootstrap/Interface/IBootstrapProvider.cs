using Godot;
using System.Threading.Tasks;

public interface IBootstrapProvider
{
    Task GoToMainMenuScene();

    Task GoToHubScene();
    void GoToOtherScene(string _sceneName);
    Node GetTargetSceneNode();
}
