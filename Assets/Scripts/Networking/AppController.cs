using System.Threading.Tasks;
using UnityEngine;

public class AppController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _ClientPrefab;
    [SerializeField] private HostSingleton _HostPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            HostSingleton hosttSingleton = Instantiate(_HostPrefab);

            hosttSingleton.CreateHost();

            await Task.Delay(200);

            ClientSingleton clientSingleton = Instantiate(_ClientPrefab);

            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated)
            {
                clientSingleton.GameManager.GoToMainMenu();
            }
        }
    }
}
