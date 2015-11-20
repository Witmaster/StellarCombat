using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
    public GameObject player1;
    public GameObject player2;
    public GameObject star;
    public GUIText feedback;
    public GUIText player1info;
    public GUIText player2info;
    public GameObject rules;
    public GameObject story;
    public GameObject restartText;
    public Player1Controller player1controller;
    public Player2Control player2controller;
    public float gravityrate = 1f;
    private bool p1FTL = false; // if this true and player object ==  null - player flew away
    private bool p2FTL = false;
    // Use this for initialization
    void Start () {
        isGameOver = false;
        restart = true;        
        player1controller = player1.GetComponent<Player1Controller>();
        player2controller = player2.GetComponent<Player2Control>();
	}
    private bool isGameOver = false;
    private bool restart = false;
    // Update is called once per frame
    void Update()
    {
        if(restart)
        {
            if (Input.inputString.toCharArray()[0] > '0' && Input.inputString.toCharArray()[0] <= '9')
            	gravityrate = int.Parse(Input.inputString.toCharArray()[0]);
            if (Input.GetKey(KeyCode.R))
            {
                player1.SetActive(true);
                player2.SetActive(true);
                star.SetActive(true);
                restart = false;
                restartText.SetActive(false);
                rules.SetActive(false);
                story.SetActive(false);
                player1controller.AdjustGravity(gravityrate); // scale gravity simultaneously
                player2controller.AdjustGravity(gravityrate);
            }
            else
            {
                player1.SetActive(false);
                player2.SetActive(false);
                star.SetActive(false);
            }
        }
        else
{
            if (!isGameOver) 
            {
                if (player1 == null && player2 == null)
                {
                    GameOver(3);
                    return;
                }
                if (player2 == null)
                {
                    GameOver(1);
                    return;
                }
                if (player1 == null)
                {
                    GameOver(2);
                    return;
                }
                if (player1controller.FTLReady() && player2controller.FTLReady() && (!p1FTL || !p2FTL))
                {
                    feedback.text = "Both players have charged their batteries and free to go";
                    p1FTL = true;
                    p2FTL = true;
                    return;
                }
                if (!p2FTL && player2controller.FTLReady())
                {
                    p2FTL = true;
                    feedback.text = "Player 2 have enough charge to activate FTL engine and escape!";
                    return;
                }
                if (player1controller.FTLReady() && !p1FTL)
                {
                    p1FTL = true;
                    feedback.text = "Player 1 has enough charge to leave this system. Press S to activate FTL engine!";
                    return;
                }
                if (player1controller.isAway && player2controller.isAway)
                {
                    GameOver(6);
                    return;
                }
                if (player2controller.isAway)
                {
                    GameOver(5);
                    return;
                }
                if (player1controller.isAway)
                {
                    GameOver(4);
                    return;
                }
                player1info.text = player1controller.GetCharge().ToString() + "%";
                player2info.text = player2controller.GetCharge().ToString() + "%";
            }
            else
            {
                if (Input.GetKey(KeyCode.R))
                    Application.LoadLevel(Application.loadedLevel);
            }
        }
        }
    public void GameOver(int winner) // 1 p1 wins, 2 - p2 wins, 3 - tie, 4 - p1 escapes, 5 - p2 escapes, 6 - both escape at once
    {
        isGameOver = true;
        if (player1 != null)
            player1controller.gameObject.SetActive(false);
        if (player2 != null)
            player2controller.gameObject.SetActive(false);
        star.SetActive(false);
        feedback.text = "Game Over\n";
        restartText.SetActive(true);
        switch (winner)
        {
            case 1:
                {
                    feedback.text += "Player 1 won the battle!";
                    break;
                }
            case 2:
                {
                    feedback.text += "Player 2 won the battle!";
                    break;
                }
            case 3:
                {
                    feedback.text += "No one survived this pointless massacre";
                    break;
                }
            case 4:
                {
                    feedback.text += "Player 1 turned this fight into race!";
                    break;
                }
            case 5:
                {
                    feedback.text += "Player 2 hit the road!";
                    break;
                }
            case 6:
                {
                    feedback.text += "Pointless battle ended as it should've been!";
                    break;
                }
            default:
                break;
        }
        }
}
