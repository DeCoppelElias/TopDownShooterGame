using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVPManager : MonoBehaviour
{
    private Player player1;
    private Player player2;

    private GameObject winUI;
    private Text winText;

    private bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.Find("Player 1").GetComponent<Player>();
        player2 = GameObject.Find("Player 2").GetComponent<Player>();

        winUI = GameObject.Find("WinUI");
        winText = winUI.transform.Find("Title").GetComponent<Text>();
        winUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDied(Player player)
    {
        winUI.SetActive(true);
        done = true;
        if (player == player1)
        {
            winText.text = "Player 2 won!";
        }
        else if (player == player2)
        {
            winText.text = "Player 1 won!";
        }
    }
}
