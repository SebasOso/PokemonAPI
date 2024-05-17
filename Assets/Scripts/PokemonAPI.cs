using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PokemonAPI : MonoBehaviour
{
    public static PokemonAPI Instance { get; private set; }
    private const string baseURL = "https://pokeapi.co/api/v2/pokemon/";

    [SerializeField] GameObject abilitiesContent;
    [SerializeField] GameObject abilityPrefab;
    [SerializeField] GameObject typesContent;
    [SerializeField] GameObject typePrefab;
    [SerializeField] TMP_InputField inputName;
    [SerializeField] TextMeshProUGUI pokemonName;
    [SerializeField] Image pokemonImage;
    public void CreatePokemon(Pokemon pokemon)
    {
        foreach (Transform child in abilitiesContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in typesContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var pokeAbility in pokemon.abilities)
        {
            GameObject ability = Instantiate(abilityPrefab, abilitiesContent.transform);
            ability.GetComponent<Ability>().CreateAbility(pokeAbility.ability.name);
        }
        foreach (var pokeType in pokemon.types)
        {
            GameObject type = Instantiate(typePrefab, typesContent.transform);
            type.GetComponent<Type>().CreateType(pokeType.type.name);
        }
        pokemonName.text = pokemon.name;
        StartCoroutine(DownloadPokemonImage(pokemon.sprites.front_default, texture =>
        {
            if (texture != null)
            {
                pokemonImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }));
    }
    public void SearchPokemon()
    {
        PokemonAPI.Instance.SearchPokemon(inputName.text);
    }
    public void SearchPokemon(string nombrePokemon)
    {
        StartCoroutine(GetPokemonData(nombrePokemon.ToLower()));
    }
    private IEnumerator GetPokemonData(string nombrePokemon)
    {
        string url = baseURL + nombrePokemon;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Pokemon pokemon = JsonUtility.FromJson<Pokemon>(request.downloadHandler.text);
            ShowPokemonInfo(pokemon);
        }
    }
    private void ShowPokemonInfo(Pokemon pokemon)
    {
        CreatePokemon(pokemon);

        Debug.Log("Habilidades:");
        foreach (var abilityInfo in pokemon.abilities)
        {
            Debug.Log(abilityInfo.ability.name);
        }

        Debug.Log("Tipos:");
        foreach (var typeInfo in pokemon.types)
        {
            Debug.Log(typeInfo.type.name);
        }
    }
    private IEnumerator DownloadPokemonImage(string imageUrl, System.Action<Texture2D> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error descargando la imagen: " + request.error);
            callback?.Invoke(null); // Llamar al callback con null en caso de error
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            callback?.Invoke(texture); // Llamar al callback con la textura descargada
        }
    }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        SearchPokemon("pikachu");
    }
    // Clase para deserializar la respuesta JSON
    [System.Serializable]
    public class Pokemon
    {
        public string name;
        public AbilityInfo[] abilities;
        public TypeInfo[] types;
        public SpriteInfo sprites;

        [System.Serializable]
        public class AbilityInfo
        {
            public Ability ability;

            [System.Serializable]
            public class Ability
            {
                public string name;
            }
        }

        [System.Serializable]
        public class TypeInfo
        {
            public Type type;

            [System.Serializable]
            public class Type
            {
                public string name;
            }
        }
        [System.Serializable]
        public class SpriteInfo
        {
            public string front_default;
        }
    }
}
