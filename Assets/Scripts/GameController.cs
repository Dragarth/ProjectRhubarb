using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public GameObject developerConsole;
    bool devConsoleActive = false;
    public Text previousCommands;
    public GameObject devConInputField;
    string commandsHelp;

    [SerializeField]
    private Transform[] spawnPoint;
    [SerializeField]
    private Transform playerHolder;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject bossPrefab;
    [SerializeField]
    Transform[] players = new Transform[4];

    [SerializeField]
    private GameObject platformHolder;

    [SerializeField]
    private Image[] currentHealth;

    [SerializeField]
    private GameObject[] winStates;


    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                GameObject boss = Instantiate(bossPrefab, spawnPoint[i].transform.position, spawnPoint[i].transform.rotation) as GameObject;
                boss.GetComponent<BossController>().controllerNumber = (i + 1);
                boss.GetComponent<BossController>().gC = this;
                boss.transform.SetParent(playerHolder);
                players[i] = boss.transform;
            }
            else
            {
                GameObject player = Instantiate(playerPrefab, spawnPoint[i].transform.position, spawnPoint[i].transform.rotation) as GameObject;
                player.GetComponent<DroneController>().controllerNumber = (i + 1);
                player.GetComponent<DroneController>().gC = this;
                player.transform.SetParent(playerHolder);
                players[i] = player.transform;
            }
        }
        SetCommandsHelpString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (devConsoleActive)
            {
                Time.timeScale = 1;
                developerConsole.SetActive(false);
                devConsoleActive = false;
            }
            else if (!devConsoleActive)
            {
                Time.timeScale = 0;
                developerConsole.SetActive(true);
                devConsoleActive = true;
                EventSystem.current.SetSelectedGameObject(developerConsole);
                EventSystem.current.SetSelectedGameObject(devConInputField);
            }
        }
    }

    public void UpdatePlayerHealth(int player, float maxHealth, float currentHealth)
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            this.players[(player - 1)].gameObject.SetActive(false);
        }
        this.currentHealth[(player - 1)].fillAmount = currentHealth / maxHealth;

        CheckWinState();
    }

    private void CheckWinState()
    {
        if (currentHealth[0].fillAmount == 0)
        {
            winStates[1].SetActive(true);
        }
        if (currentHealth[1].fillAmount == 0 && currentHealth[2].fillAmount == 0 && currentHealth[3].fillAmount == 0)
        {
            winStates[0].SetActive(true);
        }

    }

    void ResetGame(bool fly, bool twin)
    {
        players[0].GetComponent<BossController>().ResetPlayer();
        players[0].gameObject.SetActive(true);
        for (int i = 1; i < 4; i++)
        {
            players[i].GetComponent<DroneController>().ResetPlayer(fly, twin);
            players[i].gameObject.SetActive(true);
        }
        if(fly)
        {
            platformHolder.SetActive(false);
        }
        else
        {
            platformHolder.SetActive(true);
        }
        winStates[0].SetActive(false);
        winStates[1].SetActive(false);
    }

    public void DeveloperConsole(string command)
    {
        if (command == "`")
        {
            devConInputField.GetComponent<InputField>().text = "";
        }
        else
        {
            if (command.Contains("/help"))
            {
                previousCommands.text = previousCommands.text + "\n" + commandsHelp;
            }
            else if (command.Contains("/reset"))
            {
                if (command.Contains("false false"))
                    ResetGame(false, false);
                else if (command.Contains("false true"))
                    ResetGame(false, true);
                else if (command.Contains("true false"))
                    ResetGame(true, false);
                else if (command.Contains("true true"))
                    ResetGame(true, true);
                else
                    ResetGame(false, false);
            }

            previousCommands.text = previousCommands.text + "\n" + command;
            devConInputField.GetComponent<InputField>().text = "";
            EventSystem.current.SetSelectedGameObject(developerConsole);
            EventSystem.current.SetSelectedGameObject(devConInputField);
        }
    }

    void SetCommandsHelpString()
    {
        commandsHelp = "reset bool bool (fly, twinstick)"// +
            //"ui (on/off)\n" +
            //"give (1/2/3/4) (autogun/balloon/glove/mine/monkey/oil/rocket/shield/stungun/tesla/thruster/windblower)\n" +
            //"freecam off || freecam on\n" +
            //"freeze (all/1/2/3/4) || unfreeze (all/1/2/3/4)\n" +
            //"restart"
            ;
    }
}
