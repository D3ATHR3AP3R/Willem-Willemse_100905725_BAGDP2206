using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _MainUI;
    [SerializeField] private GameObject _JoinUi;

    public TMP_InputField joinCodeInput;

    private void Start()
    {
        _MainUI.SetActive(true);
        _JoinUi.SetActive(false);
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.startHostAsync();
    }

    public void JoinUi()
    {
        _JoinUi.SetActive(true);
        _MainUI.SetActive(false);
    }

    public async void StartClient(string joinCode)
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeInput.text);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
