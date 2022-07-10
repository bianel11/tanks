using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 3; //Número de rondas que un jugador debe ganar para ganar el juego 
    public float m_StartDelay = 3f; ///Delay entre las fases de RoundStarting yRoundPlaying
    public float m_EndDelay = 3f; ///Delay entre las fases de RoundPlaying y RoundEnding
    public CameraControl m_CameraControl; ///Referencia al sccript de CameraControl 
    public Text m_MessageText; //Referencia al texto para mostrar mensajes
    public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;


    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private bool isEnd = false;

    const float k_MaxDepenetrationVelocity = float.PositiveInfinity;

    private void Start()
    {
        // This line fixes a change to the physics engine.
        Physics.defaultMaxDepenetrationVelocity = k_MaxDepenetrationVelocity;

        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {

        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (!isEnd)
        {
            if (m_GameWinner != null)
            {
                SceneManager.LoadScene(0);
            }
            else
            {

                StartCoroutine(GameLoop());
            }
        }

    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND" + m_RoundNumber;


        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        // Cuando empiece la ronda dejo que los tanques se muevan.

        EnableTankControl();
        // Borro el texto de la pantalla
        m_MessageText.text = string.Empty;
        // Mientras haya más de un tanque...

        while (!OneTankLeft())
        {
            // ... vuelvo al frame siguiente.
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
        {
            m_RoundWinner.m_Wins++;
        }

        m_GameWinner = GetGameWinner();
        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
        {
            message = m_GameWinner.m_ColoredPlayerText + " GANO EL JUEGO!\n";
            message += "PUNTUAJE: " + m_GameWinner.m_Wins;
            isEnd = true;
        }

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}