using UnityEngine;
using TMPro;

public class CharacterCustomizer : MonoBehaviour
{
    public Transform characters;
    public TMP_InputField inputName;

    private int selectedIndex;
    private bool nameChanged;
    private string randomName;

    private void Start()
    {
        InitInputName();
    }

    public void Click(int index)
    {
        selectedIndex = index;

        for (int i = 0; i < characters.childCount; i++)
        {
            characters.GetChild(i).GetChild(0).gameObject.SetActive(index == i);
            characters.GetChild(i).GetChild(1).gameObject.SetActive(index != i);
        }        
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    public void InitInputName()
    {
        var jsonTextFile = Resources.Load<TextAsset>("nicknames");
        Nicknames nicknames = JsonUtility.FromJson<Nicknames>(jsonTextFile.text);
        randomName = nicknames.nicknames[Random.Range(0, nicknames.nicknames.Count)];

        if (PlayerPrefs.HasKey(Utils.gamePrefix + "name"))
        {
            inputName.text = PlayerPrefs.GetString(Utils.gamePrefix + "name");
        }
        else
        {
            inputName.text = randomName;
        }
    }

    public string GetNickName()
    {
        if (string.IsNullOrWhiteSpace(inputName.text))
        {
            PlayerPrefs.DeleteKey(Utils.gamePrefix + "name");
            return randomName;
        }

        if (nameChanged)
        {
            PlayerPrefs.SetString(Utils.gamePrefix + "name", inputName.text);
        }

        return inputName.text;
    }

    public void OnNameChanged()
    {
        nameChanged = true;
    }
}
