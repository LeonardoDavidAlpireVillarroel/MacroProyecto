using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    [Header("Base de datos de ítems")]
    public ItemsDataBase itemsDataBase;
    private Dictionary<int, int> itemIDsRecolectados = new Dictionary<int, int>();

    [Header("CanvasGroup para fade")]
    public CanvasGroup resultadosCanvasGroup;

    [Header("Botones")]
    public Button botonContinuarResultados;

    [Header("Paneles de resultado")]
    public GameObject panelDesbloqueo;
    public GameObject panelSinDesbloqueo;

    private bool isFading = false;
    private Collider triggerToActivate;

    [Header("Objetivos del Nivel")]
    public List<GameObject> enemigosEnNivel = new List<GameObject>();
    public List<GameObject> itemsEnNivel = new List<GameObject>();

    [Header("IDs obligatorios para desbloqueo")]
    public List<GameObject> itemsNecesariosParaDesbloqueo = new List<GameObject>();
    [Header("UI Progreso Items Necesarios")]
    public TextMeshProUGUI textoProgresoItemsNecesarios;

    private int totalEnemigosEnNivel;
    private int totalItemsEnNivel;

    private int enemigosDerrotados = 0;
    private int itemsRecolectados = 0;

    [Header("Intro")]
    public GameObject introPanel;
    public CanvasGroup introCanvasGroup;
    public Button botonContinuar;

    [Header("Dash Tutorial")]
    public DashTutorialController dashTutorialController;

    [Header("Temporizador")]
    public float tiempoLimite = 121f; // 2 minutos (120) y un poco mas
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
    public TextMeshProUGUI textoPuntosTiempo;
    public TextMeshProUGUI textoPuntosTotales;

    private bool nivelFinalizado = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        totalEnemigosEnNivel = enemigosEnNivel.Count;
        totalItemsEnNivel = itemsEnNivel.Count;

        tiempoRestante = tiempoLimite;

        if (SceneManager.GetActiveScene().name == "Level2" && dashTutorialController != null)
        {
            int dashTutorialSeen = ProfileStorage.s_currentProfile.GetPrefInt("dashTutorialSeen", 0);
            if (dashTutorialSeen == 0)
            {
                // Mostrar dash tutorial
                dashTutorialController.gameObject.SetActive(true);
                dashTutorialController.CheckAndShowDashTutorial();

                // Pausar tiempo desde el inicio
                Time.timeScale = 0;

                // Suscribimos evento Close del dashTutorial (debería tener un botón Close)
                dashTutorialController.closeButton.onClick.RemoveAllListeners();
                dashTutorialController.closeButton.onClick.AddListener(() =>
                {
                    dashTutorialController.gameObject.SetActive(false);
                    ProfileStorage.s_currentProfile.SetPrefInt("dashTutorialSeen", 1);
                    MostrarIntroPanelConTiempoPausado();
                });
            }
            else
            {
                // Si ya se vio el tutorial, mostrar intro normal y arrancar tiempo normalmente
                StartCoroutine(EsperarYCargarIntro());
            }
        }
        else
        {
            // Otros niveles
            StartCoroutine(EsperarYCargarIntro());
        }

        // Botón continuar en el intro panel
        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(() =>
            {
                CerrarIntroYReanudarTiempo();
            });
        }

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

        ActualizarProgresoItemsNecesarios();
    }

    public void StartIntroAfterDash()
    {
        StartCoroutine(EsperarYCargarIntro());
    }

    IEnumerator EsperarYCargarIntro()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(FadeInIntroPanel());
    }

    IEnumerator FadeInIntroPanel()
    {
        Time.timeScale = 0;

        float duracion = 1f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            introCanvasGroup.alpha = Mathf.Lerp(0f, 1f, tiempo / duracion);
            yield return null;
        }

        introCanvasGroup.alpha = 1f;
        introCanvasGroup.interactable = true;
        introCanvasGroup.blocksRaycasts = true;
    }

    void IniciarNivel()
    {
        if (introCanvasGroup != null)
        {
            StartCoroutine(FadeOutIntroPanel());
        }
    }

    public void ForzarInicioNivelDesdeDashTutorial()
    {
        StartCoroutine(FadeOutIntroPanel());
    }

    IEnumerator FadeOutIntroPanel()
    {
        isFading = true;
        float duracion = 1f;
        float tiempo = 0f;

        introCanvasGroup.interactable = false;
        introCanvasGroup.blocksRaycasts = false;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            introCanvasGroup.alpha = Mathf.Lerp(1f, 0f, tiempo / duracion);
            yield return null;
        }

        introCanvasGroup.alpha = 0f;

        if (introPanel != null)
            introPanel.SetActive(false);

        isFading = false;

        temporizadorActivo = true;
        Time.timeScale = 1f;
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

    public void IncrementarItemsRecolectados(int itemID)
    {
        itemsRecolectados++;

        if (itemIDsRecolectados.ContainsKey(itemID))
            itemIDsRecolectados[itemID]++;
        else
            itemIDsRecolectados[itemID] = 1;
    }

    public void IncrementarEnemigosDerrotados()
    {
        enemigosDerrotados++;
    }

    bool TodosLosObjetivosCompletados()
    {
        return itemsRecolectados >= totalItemsEnNivel && enemigosDerrotados >= totalEnemigosEnNivel;
    }

    void MostrarTiempoEnPantalla()
    {
        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void MostrarIntroPanelConTiempoPausado()
    {
        introPanel.SetActive(true);
        introCanvasGroup.alpha = 1f;
        introCanvasGroup.interactable = true;
        introCanvasGroup.blocksRaycasts = true;
    }

    private void CerrarIntroYReanudarTiempo()
    {
        StartCoroutine(FadeOutIntroPanel());
    }

    public float ObtenerTiempoRestante()
    {
        return tiempoRestante;
    }

    void ActualizarProgresoItemsNecesarios()
    {
        int itemsRecolectadosNecesarios = 0;

        foreach (GameObject itemNecesario in itemsNecesariosParaDesbloqueo)
        {
            if (collectedItems.Contains(itemNecesario))
            {
                itemsRecolectadosNecesarios++;
            }
        }

        int totalItemsNecesarios = itemsNecesariosParaDesbloqueo.Count;
        int itemsFaltantes = totalItemsNecesarios - itemsRecolectadosNecesarios;

        if (textoProgresoItemsNecesarios != null)
        {
            textoProgresoItemsNecesarios.text = $"{itemsRecolectadosNecesarios} / {totalItemsNecesarios}";
        }
    }

    public bool HasCollectedAllRequiredItems()
    {
        foreach (GameObject itemNecesario in itemsNecesariosParaDesbloqueo)
        {
            if (!collectedItems.Contains(itemNecesario))
            {
                return false;
            }
        }
        return true;
    }

    void FinalizarNivel(string mensaje)
    {
        if (nivelFinalizado) return;

        nivelFinalizado = true;
        GameManager.Instance.PauseGame();

        float tiempoFinal = tiempoRestante;

        int puntosItems = 0;
        foreach (var pair in itemIDsRecolectados)
        {
            int id = pair.Key;
            var item = itemsDataBase.GetItemByID(id);
            if (item != null)
            {
                puntosItems += item.Value.puntosAlRecoger * pair.Value;
            }
        }

        int puntosEnemigos = enemigosDerrotados * 20;
        int puntosTiempo = Mathf.FloorToInt(tiempoFinal / 5) * 5;
        int puntosTotales = puntosItems + puntosEnemigos + puntosTiempo;

        GameManager.Instance.SumarPuntos(puntosTotales);
        GameManager.Instance.OnLevelCompleted();

        if (textoResultado != null)
            textoResultado.text = mensaje;

        if (textoItems != null)
            textoItems.text = $"{itemsRecolectados}/{totalItemsEnNivel}";

        if (textoEnemigos != null)
            textoEnemigos.text = $"{enemigosDerrotados}/{totalEnemigosEnNivel}";

        if (textoTiempo != null)
        {
            int minutos = Mathf.FloorToInt(tiempoFinal / 60f);
            int segundos = Mathf.FloorToInt(tiempoFinal % 60f);
            textoTiempo.text = $"{minutos:00}:{segundos:00}";
        }

        if (textoPuntosItems != null)
            textoPuntosItems.text = $"{puntosItems}";

        if (textoPuntosEnemigos != null)
            textoPuntosEnemigos.text = $"{puntosEnemigos}";

        if (textoPuntosTiempo != null)
            textoPuntosTiempo.text = $"{puntosTiempo}";

        if (textoPuntosTotales != null)
            textoPuntosTotales.text = $"{puntosTotales}";

        bool recogioTodo = HasCollectedAllRequiredItems();

        if (panelResultados != null)
            panelResultados.SetActive(true);

        if (panelDesbloqueo != null) panelDesbloqueo.SetActive(false);
        if (panelSinDesbloqueo != null) panelSinDesbloqueo.SetActive(false);

        if (recogioTodo)
        {
            if (panelDesbloqueo != null)
            {
                panelDesbloqueo.SetActive(true);
            }
        }
        else
        {
            if (panelSinDesbloqueo != null)
            {
                panelSinDesbloqueo.SetActive(true);
                if (textoResultado != null)
                {
                    textoResultado.text += "\n\nNo recogiste todos los objetos clave.\nDebes reintentar o volver al Claro.";
                }
            }
        }

        StartCoroutine(FadeInResultados());
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

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ClaroPacifico")
            {
                MapController map = FindFirstObjectByType<MapController>();
                if (map != null)
                {
                    map.MostrarPanelMapaAutomaticamente();
                }
            }
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
        Time.timeScale = 1;
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

    public void EliminarItemsRecolectadosDelInventario()
    {
        Inventory inv = Inventory.Instance;
        if (inv == null) return;

        foreach (var kvp in itemIDsRecolectados)
        {
            inv.DeleteItem(kvp.Key, kvp.Value);
        }

        itemIDsRecolectados.Clear();
    }

    private List<GameObject> collectedItems = new List<GameObject>();
    private List<GameObject> collectedItemsBackup = new List<GameObject>();
    private List<int> collectedItemIDs = new List<int>();

    public List<int> GetCollectedItemIDs()
    {
        return collectedItemIDs;
    }

    public void AddCollectedItem(GameObject item)
    {
        if (!collectedItems.Contains(item))
        {
            collectedItems.Add(item);
            ActualizarProgresoItemsNecesarios();
        }
    }

    public void BackupCollectedItems()
    {
        collectedItemsBackup = new List<GameObject>(collectedItems);
    }

    public void RestoreCollectedItems()
    {
        foreach (GameObject item in collectedItems)
        {
            if (item != null && !collectedItemsBackup.Contains(item))
            {
                item.SetActive(true);
            }
        }

        collectedItems = new List<GameObject>(collectedItemsBackup);
    }

    public void ClearCollectedItems()
    {
        collectedItemIDs.Clear();
    }

    public void IncrementarItemsRecolectados2(int id)
    {
        if (!collectedItemIDs.Contains(id))
        {
            collectedItemIDs.Add(id);
        }
    }
}