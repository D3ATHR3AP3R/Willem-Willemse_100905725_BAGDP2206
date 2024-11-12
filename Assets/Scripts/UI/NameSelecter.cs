using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelecter : MonoBehaviour
{
    [SerializeField] private TMP_InputField _NameField;
    [SerializeField] private Button _ConnectBtn;
    [SerializeField] private int _MinNameLength = 1;
    [SerializeField] private int _MaxNameLength = 12;

    public const string _PlayerNameKey = "PlayerName";
    private const string _SceneToLoad = "NetBootstrap";

    private void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(_SceneToLoad);
            return;
        }

        _NameField.text = PlayerPrefs.GetString(_PlayerNameKey, string.Empty);

        HandleNameChange();
    }

    public void HandleNameChange()
    {
        _ConnectBtn.interactable = _NameField.text.Length >= _MinNameLength &&
            _NameField.text.Length <= _MaxNameLength;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(_PlayerNameKey, _NameField.text);
        SceneManager.LoadScene(_SceneToLoad);
    }
}
