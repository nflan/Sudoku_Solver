using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;


public class ValidateInput : MonoBehaviour
{
    [Header("ValidateInput Information")]
    [SerializeField] private Square m_Square = null;
    [SerializeField] private TMP_InputField m_InputFieldTxt = null;
    [SerializeField] private GameEngine m_GameEngine = null;

    [Header("Error Typo Information")]
    [SerializeField] private ErrorTypo m_ErrorPanel = null;

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
        if (!m_GameEngine)
        {
            m_GameEngine = FindAnyObjectByType<GameEngine>();
            if (!m_GameEngine)
            {
                Debug.LogError("No GameEngine in the scene.");
                Application.Quit();
            }
        }
        if (!m_ErrorPanel)
        {
            m_ErrorPanel = this.GetComponentInChildren<ErrorTypo>(true);
            if (!m_ErrorPanel)
            {
                Debug.LogError("No Error Panel in the scene.");
                Application.Quit();
            }
            m_ErrorPanel.gameObject.SetActive(false);
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

    private bool IsDuplicatedValueBlock(int index, int newValue)
    {
        foreach (Square square in m_GameEngine.GetBlock(m_GameEngine.GetBlockIndex(index)))
        {
            if (int.TryParse(square.GetValue(), out int result))
            {
                if (result == newValue)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsDuplicatedValueColumn(int index, int newValue)
    {
        foreach (Square square in m_GameEngine.GetColumn(index % 9))
        {
            if (int.TryParse(square.GetValue(), out int result))
            {
                if (result == newValue)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsDuplicatedValueRow(int index, int newValue)
    {
        foreach (Square square in m_GameEngine.GetRow(index / 9))
        {
            if (int.TryParse(square.GetValue(), out int result))
            {
                if (result == newValue)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsDuplicatedValues(int index, int newValue)
    {
        if (IsDuplicatedValueBlock(index, newValue) || IsDuplicatedValueColumn(index, newValue) || IsDuplicatedValueRow(index, newValue))
        {
            return true;
        }
        return false;
    }

    public void InputKey(string key)
    {
        if (!this.m_Square)
        {
            HideInputValue();
            return ;
        }
        if (key.Length == 1)
        {
            if (int.TryParse(key, out int result) && result != 0)
            {
                // Add a check to ensure the input is a valid number (no same number in line/column/square)
                if (!this.IsDuplicatedValues(m_Square.GetIndex(), result))
                {
                    this.m_Square.SetValue(key);
                    HideInputValue();
                    return ;
                }
                else
                {
                    m_ErrorPanel.gameObject.SetActive(false);
                    m_ErrorPanel.SetErrorTxt("This number already exists in the same row, column or block.");
                    m_ErrorPanel.gameObject.SetActive(true);
                }
            }
            else
            {
                m_ErrorPanel.gameObject.SetActive(false);
                m_ErrorPanel.SetErrorTxt("Invalid input. Please enter a number between 1 and 9.");
                m_ErrorPanel.gameObject.SetActive(true);
            }
        }
        if (m_InputFieldTxt.text.Length > 0)
        {
            m_InputFieldTxt.text = "";
        }
    }

    public void HideInputValue()
    {
        this.gameObject.SetActive(false);
        this.m_Square = null;
    }
}
