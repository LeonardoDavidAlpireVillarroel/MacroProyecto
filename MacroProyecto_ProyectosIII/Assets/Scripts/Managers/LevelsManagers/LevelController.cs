using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LevelController : MonoBehaviour
{
    [Header("CanvasGroup para fade")]
    public CanvasGroup resultadosCanvasGroup;

    [Header("Botón continuar")]
    public Button botonContinuarResultados;

    private bool isFading = false;
    private Collider triggerToActivate;

    [Header("Objetivos del Nivel")]
    public List<GameObject> enemigosEnNivel = new List<GameObject>();
    public List<GameObject> itemsEnNivel = new List<GameObject>();

    private int totalEnemigosEnNivel;
    private int totalItemsEnNivel;

    private int enemigosDerrotados = 0;
    private int frutasRecolectadas = 0;

    [Header("Intro")]
    public GameObject introPanel;
    public Button botonContinuar;

    [Header("Temporizador")]
    public float tiempoLimite = 120f; // 2 minutos
    private float tiempoRestante;
    private bool temporizadorActivo = false;

    public TextMeshProUGUI timerText;

    [Header("Panel final")]
    public GameObject panelResultados;
    public TextMeshProUGUI textoResultado;
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoItems;
    public TextMeshProUGUI textoEnemigos;
    public TextMeshProUGUI textoPuntosItems;
    public TextMeshProUGUI textoPuntosEnemigos;
    public TextMeshProUGUI textoPuntosTiempo;    // Nuevo TMP para puntos por tiempo
    public TextMeshProUGUI textoPuntosTotales;

    private bool nivelFinalizado = false;


    void Start()
    {
        totalEnemigosEnNivel = enemigosEnNivel.Count;
        totalItemsEnNivel = itemsEnNivel.Count;

        tiempoRestante = tiempoLimite;

        if (introPanel != null)
            introPanel.SetActive(true);

        if (botonContinuar != null)
            botonContinuar.onClick.AddListener(IniciarNivel);

        StartCoroutine(PausarConRetraso());

        if (panelResultados != null)
        {
            panelResultados.SetActive(false);
        }

        if (botonContinuarResultados != null)
        {
            botonContinuarResultados.gameObject.SetActive(false);
            botonContinuarResultados.onClick.AddListener(OnClickContinuarResultados);
        }

        triggerToActivate = GetComponent<Collider>();
        if (triggerToActivate != null)
        {
            triggerToActivate.enabled = false;
        }
    }

    IEnumerator PausarConRetraso()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.PauseGame();
    }

    void IniciarNivel()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        GameManager.Instance.ResumeGame();
        temporizadorActivo = true;
    }

    void Update()
    {
        if (!temporizadorActivo || nivelFinalizado) return;

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            temporizadorActivo = false;
            FinalizarNivel("¡Se acabó el tiempo!");
        }

        MostrarTiempoEnPantalla();

        if (TodosLosObjetivosCompletados())
        {
            temporizadorActivo = false;
            FinalizarNivel("¡Objetivos completados!");
        }
    }

    public void IncrementarItemsRecolectados()
    {
        frutasRecolectadas++;
    }

    public void IncrementarEnemigosDerrotados()
    {
        enemigosDerrotados++;
    }

    bool TodosLosObjetivosCompletados()
    {
        return frutasRecolectadas >= totalItemsEnNivel && enemigosDerrotados >= totalEnemigosEnNivel;
    }

    void MostrarTiempoEnPantalla()
    {
        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    public float ObtenerTiempoRestante()
    {
        return tiempoRestante;
    }

    void FinalizarNivel(string mensaje)
    {
        if (nivelFinalizado) return;

        nivelFinalizado = true;
        GameManager.Instance.PauseGame();
        GameManager.Instance.OnLevelCompleted();

        if (panelResultados != null && resultadosCanvasGroup != null)
        {
            panelResultados.SetActive(true);
            botonContinuarResultados.gameObject.SetActive(false);
            resultadosCanvasGroup.alpha = 0f;
            resultadosCanvasGroup.interactable = false;
            resultadosCanvasGroup.blocksRaycasts = false;

            if (textoResultado != null)
                textoResultado.text = mensaje;

            float tiempoEmpleado = tiempoLimite - tiempoRestante;
            int minutos = Mathf.FloorToInt(tiempoEmpleado / 60f);
            int segundos = Mathf.FloorToInt(tiempoEmpleado % 60f);
            if (textoTiempo != null)
                textoTiempo.text = $"{minutos:00}:{segundos:00}";

            if (textoItems != null)
                textoItems.text = $"{frutasRecolectadas} / {totalItemsEnNivel}";

            if (textoEnemigos != null)
                textoEnemigos.text = $"{enemigosDerrotados} / {totalEnemigosEnNivel}";

            int puntosItems = frutasRecolectadas * 10;
            int puntosEnemigos = enemigosDerrotados * 20;
            int puntosTiempo = ((int)(tiempoRestante / 5)) * 5;

            if (textoPuntosItems != null)
                textoPuntosItems.text = puntosItems.ToString();

            if (textoPuntosEnemigos != null)
                textoPuntosEnemigos.text = puntosEnemigos.ToString();

            if (textoPuntosTiempo != null)
                textoPuntosTiempo.text = puntosTiempo.ToString();

            int puntosTotales = puntosItems + puntosEnemigos + puntosTiempo;
            if (textoPuntosTotales != null)
                textoPuntosTotales.text = puntosTotales.ToString();

            StartCoroutine(FadeInResultados());
        }
    }

    IEnumerator FadeInResultados()
    {
        isFading = true;
        float duracion = 1f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            resultadosCanvasGroup.alpha = Mathf.Lerp(0f, 1f, tiempo / duracion);
            yield return null;
        }

        resultadosCanvasGroup.alpha = 1f;
        resultadosCanvasGroup.interactable = true;
        resultadosCanvasGroup.blocksRaycasts = true;

        if (botonContinuarResultados != null)
            botonContinuarResultados.gameObject.SetActive(true);

        isFading = false;
    }

    public void OnClickContinuarResultados()
    {
        if (!isFading)
        {
            botonContinuarResultados.gameObject.SetActive(false);
            StartCoroutine(FadeOutResultados());
        }
    }

    IEnumerator FadeOutResultados()
    {
        isFading = true;
        float duracion = 1f;
        float tiempo = 0f;

        resultadosCanvasGroup.interactable = false;
        resultadosCanvasGroup.blocksRaycasts = false;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            resultadosCanvasGroup.alpha = Mathf.Lerp(1f, 0f, tiempo / duracion);
            yield return null;
        }

        resultadosCanvasGroup.alpha = 0f;
        panelResultados.SetActive(false);

        if (triggerToActivate != null)
        {
            triggerToActivate.enabled = true;
        }

        isFading = false;

        GameManager.Instance.ResumeGame();
    }

    public bool AreAllObjectivesCompleted()
    {
        return TodosLosObjetivosCompletados();
    }

    public bool IsTimerActive()
    {
        return temporizadorActivo;
    }

    public void FinishLevelEarly(string mensaje)
    {
        if (!nivelFinalizado)
        {
            temporizadorActivo = false;
            FinalizarNivel(mensaje);
        }
    }
}
