using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;


public class ValidateInput : MonoBehaviour
{
    [SerializeField] private Square m_Square = null;
    [SerializeField] private TMP_InputField m_InputFieldTxt = null;

    void Awake()
    {
        if (!m_InputFieldTxt)
        {
            m_InputFieldTxt = GameObject.FindGameObjectWithTag("InputFieldText").GetComponent<TMP_InputField>();
            if (!m_InputFieldTxt)
            {
                Debug.LogError("No InputFieldText in the scene.");
                Application.Quit();
            }
            m_InputFieldTxt.text = null;
        }
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            m_InputFieldTxt.ActivateInputField();
            m_InputFieldTxt.Select();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideInputValue();
            return ;
        }
    }

    void OnEnable()
    {
        if (m_InputFieldTxt)
        {
            m_InputFieldTxt.text = null;

            // Frame issue: ActivateInputField() must be called in the next frame
            // to ensure the input field is ready to receive input.
            StartCoroutine(ActivateInputFieldNextFrame());
        }
    }

    void OnDisable()
    {
        if (m_InputFieldTxt)
        {
            m_InputFieldTxt.text = null;
            SetCursorVisible(true);
        }
    }

    private void SetCursorVisible(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    private IEnumerator ActivateInputFieldNextFrame()
    {
        yield return null; // Wait for the next frame
        SetCursorVisible(false);
        m_InputFieldTxt.ActivateInputField();
        m_InputFieldTxt.Select();
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
            if (m_InputFieldTxt.text.Length > 0)
            {
                m_InputFieldTxt.text = "";
            }
        }

    }

    public void HideInputValue()
    {
        this.gameObject.SetActive(false);
        this.m_Square = null;
    }
}
