using System.Collections;
using UnityEngine;
using TMPro;

public class WritePhrase : MonoBehaviour
{
    public string[] phrases; // Array to hold multiple phrases
    private int currentPhraseIndex = 0; // Index to keep track of the current phrase
    public TextMeshProUGUI texto; // Use TextMeshProUGUI for UI text elements
    public GameObject ucumar;
    public GameObject parpadoArriba;
    public GameObject parpadoAbajo;
    public GameObject mensaje;
    private Animator animParpadoArriba;
    private Animator animParpadoAbajo;
    private Animator animMensaje;
    private bool isTyping = false;
    private bool isCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        animParpadoArriba = parpadoArriba.GetComponent<Animator>();
        animParpadoAbajo = parpadoAbajo.GetComponent<Animator>();
        animMensaje = mensaje.GetComponent<Animator>();
        if (phrases.Length > 0)
        {
            StartCoroutine(TypeText(phrases[currentPhraseIndex]));
        }
    }

    void Update()
    {
        if (phrases.Length == 0)
        {
            animParpadoArriba.SetTrigger("upEyeSpeed");
            animParpadoAbajo.SetTrigger("downEyeSpeed");
            animMensaje.SetTrigger("activeMensaje");
        }
        else
        {

            if (Input.GetMouseButtonDown(0)) // Detect click anywhere on the screen
            {
                if (isTyping)
                {
                    // Finish the current text immediately
                    StopAllCoroutines();
                    texto.text = phrases[currentPhraseIndex];
                    isTyping = false;
                    isCompleted = true;
                }
                else if (isCompleted)
                {
                    if (currentPhraseIndex < phrases.Length - 1)
                    {
                        currentPhraseIndex++;
                        texto.text = ""; // Clear the current text
                        StartCoroutine(TypeText(phrases[currentPhraseIndex])); // Start typing the next phrase
                    }
                    else
                    {
                        // All phrases have been shown, trigger animation and disable text

                        animParpadoArriba.SetTrigger("upEye");
                        animParpadoAbajo.SetTrigger("downEye");
                        texto.enabled = false;
                        ucumar.SetActive(true);
                    }
                    isCompleted = false;
                }
            }
        }
    }

    IEnumerator TypeText(string frase)
    {
        isTyping = true;
        foreach (char caracter in frase)
        {
            texto.text += caracter;
            yield return new WaitForSeconds(0.08f);
        }
        isTyping = false;
        isCompleted = true;
    }
}
