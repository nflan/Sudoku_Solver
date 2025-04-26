using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class ValidateInput : MonoBehaviour
{
    [SerializeField] private Square m_Square = null;
    [SerializeField] private TMP_Text m_InputFieldTxt = null;

    void Start()
    {
        if (!m_InputFieldTxt)
        {
            m_InputFieldTxt = GameObject.FindGameObjectWithTag("InputFieldText").GetComponent<TMP_Text>();
            if (!m_InputFieldTxt)
            {
                Debug.LogError("No InputFieldText in the scene.");
                Application.Quit();
            }
            m_InputFieldTxt.text = null;
        }
    }

    void OnEnable()
    {
    }

    public void SetSquare(Square square)
    {
        this.m_Square = square;
    }

    public void InputKey(string key)
    {
        if (!this.m_Square)
        {
            HideInputValue();
            return ;
        }
        if (key.Length == 1 && int.TryParse(key, out int result))
        {
            this.m_Square.SetValue(key);
            HideInputValue();
            return ;
        }
        else
        {
            string change = m_InputFieldTxt.text;
            change = "";
            m_InputFieldTxt.text = change;
            m_InputFieldTxt.text.Remove(0);
            Debug.Log("m_InputFieldTxt.text = " + m_InputFieldTxt.text);
            Debug.Log("change = " + change);
        }

    }

    public void HideInputValue()
    {
        this.m_InputFieldTxt.text = "";
        this.gameObject.SetActive(false);
        this.m_InputFieldTxt.text = "";
        this.m_Square = null;
    }
}
