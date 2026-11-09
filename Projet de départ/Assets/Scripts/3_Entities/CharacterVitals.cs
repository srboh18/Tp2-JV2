using UnityEngine;
using Random = UnityEngine.Random;

// Signes vitaux d'un personnage.
//
// Les personnages ont trois signes vitaux : "hunger" (faim), "loneliness" (solitude) et "sleepiness" (sommeil). Plus
// la valeur est basse, et mieux c'est. Par exemple, une valeur de 0 pour "hunger" signifie que le personnage n'a pas
// faim. En revanche, une valeur de 100 signifie que le personnage meurt de faim.
//
// Cette classe ne met pas à jour automatiquement ces valeurs. Il faut appeler les méthodes "Raise" et "Lower" dans les
// états de votre "State Machine". Certains états améliorent les signes vitaux et d'autres les réduisent. Par exemple,
// si votre personne est dans l'état "Manger", alors sa faim sera réduite progressivement.
//
// Tous les signes vitaux ont une valeur considérée dangereuse, nommée "Threshold". Au dela de cette valeur, considérez
// de changer d'état du personnage pour aller combler le besoin en souffrance. Dans vos états, consultez périodiquement
// les propriétés "Is___AboveThreshold" afin d'éviter que le personnage ne meurt.
//
// Vous n'avez pas à toucher cette classe pour le travail.
public class CharacterVitals : MonoBehaviour
{
    [Header("Current values")]
    [SerializeField, Range(0, 100)] private float hunger = 0;
    [SerializeField, Range(0, 100)] private float loneliness = 0;
    [SerializeField, Range(0, 100)] private float sleepiness = 0;

    [Header("Initial values")]
    [SerializeField, Range(0, 100)] private float minStartingHunger = 0f;
    [SerializeField, Range(0, 100)] private float maxStartingHunger = 25f;
    [SerializeField, Range(0, 100)] private float minStartingLoneliness = 0f;
    [SerializeField, Range(0, 100)] private float maxStartingLoneliness = 50f;
    [SerializeField, Range(0, 100)] private float minStartingSleepiness = 0f;
    [SerializeField, Range(0, 100)] private float maxStartingSleepiness = 50f;

    [Header("Change rates")]
    [SerializeField, Min(0)] private float hungerGainRate = 1f; //2.5f;
    [SerializeField, Min(0)] private float hungerReductionRate = 25f;
    [SerializeField, Min(0)] private float lonelinessGainRate = 0.5f;
    [SerializeField, Min(0)] private float lonelinessReductionRate = 15f;
    [SerializeField, Min(0)] private float sleepinessGainRate = 1f;
    [SerializeField, Min(0)] private float sleepinessReductionRate = 10f;

    [Header("Thresholds")]
    [SerializeField, Range(0, 100)] private float hungerTarget = 0f;
    [SerializeField, Range(0, 100)] private float hungerThreshold = 50f;
    [SerializeField, Range(0, 100)] private float lonelinessTarget = 0f;
    [SerializeField, Range(0, 100)] private float lonelinessThreshold = 80f;
    [SerializeField, Range(0, 100)] private float sleepinessTarget = 0f;
    [SerializeField, Range(0, 100)] private float sleepinessThreshold = 70f;

#if UNITY_EDITOR
    private Character character;
    private bool hasDied;
#endif

    // Vitals.
    public float Hunger => hunger;
    public float Loneliness => loneliness;
    public float Sleepiness => sleepiness;

    // Thresholds.
    public bool IsHungerBelowTarget => hunger <= hungerTarget;
    public bool IsHungerAboveThreshold => hunger >= hungerThreshold;
    public bool IsLonelinessBelowTarget => loneliness <= lonelinessTarget;
    public bool IsLonelinessAboveThreshold => loneliness >= lonelinessThreshold;
    public bool IsSleepinessBelowTarget => sleepiness <= sleepinessTarget;
    public bool IsSleepinessAboveThreshold => sleepiness >= sleepinessThreshold;

    private void Awake()
    {
        hunger = Random.Range(minStartingHunger, maxStartingHunger);
        loneliness = Random.Range(minStartingLoneliness, maxStartingLoneliness);
        sleepiness = Random.Range(minStartingSleepiness, maxStartingSleepiness);

#if UNITY_EDITOR
        character = GetComponent<Character>();
        hasDied = false;
#endif
    }

#if UNITY_EDITOR
    private void Update()
    {
        // Check character vitals. If any goes above 100, log a warning in the console.
        // Do not spam the console with warnings. Give the cause of death only once.
        if (!hasDied)
        {
            if (hunger >= 100)
            {
                Debug.LogWarning($"{character.FullName} died of hunger.", gameObject);
                hasDied = true;
            }
            else if (loneliness >= 100)
            {
                Debug.LogWarning($"{character.FullName} died of loneliness.", gameObject);
                hasDied = true;
            }
            else if (sleepiness >= 100)
            {
                Debug.LogWarning($"{character.FullName} died from a lack of sleep.", gameObject);
                hasDied = true;
            }
            
        }
    }
#endif

    public void RaiseHunger()
    {
        hunger += hungerGainRate * Time.deltaTime;
        if (hunger > 100) hunger = 100;
    }

    public void RaiseLoneliness()
    {
        loneliness += lonelinessGainRate * Time.deltaTime;
        if (loneliness > 100) loneliness = 100;
    }

    public void RaiseSleepiness()
    {
        sleepiness += sleepinessGainRate * Time.deltaTime;
        if (sleepiness > 100) sleepiness = 100;
    }

    public void LowerHunger()
    {
        hunger -= hungerReductionRate * Time.deltaTime;
        if (hunger < 0) hunger = 0;
    }

    public void LowerLoneliness()
    {
        loneliness -= lonelinessReductionRate * Time.deltaTime;
        if (loneliness < 0) loneliness = 0;
    }

    public void LowerSleepiness()
    {
        sleepiness -= sleepinessReductionRate * Time.deltaTime;
        if (sleepiness < 0) sleepiness = 0;
    }
}