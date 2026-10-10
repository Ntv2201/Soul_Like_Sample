using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{

    public static PlayerUIManager instance;

    [Header("NETWORK JOIN")]
    [SerializeField] bool startGameAsClient;
    [HideInInspector] public PlayerUIHudManager playerUIHudManager;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();

    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;

            // we must first shutdown the network, cua we have started as a host during the title screen
            NetworkManager.Singleton.Shutdown();

            // then restart as a client
            NetworkManager.Singleton.StartClient();
        }
    }
    
}
