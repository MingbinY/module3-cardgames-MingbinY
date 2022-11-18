using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class GolfScoreManager : MonoBehaviour
{
    static public GolfScoreManager S;
    public static int SCORE_FROM_PREV_ROUND;
    public static int HIGH_SCORE = 54;

    public Text highScore;
    public Text RoundResult;
    public Text scoreBoard;
    public Text roundCount;
    public Text scoreText;
    public int currentScore;

    public static int currentRound = 0;
    private void Awake()
    {
        if (S == null)
            S = this;

        RoundManager._instance.roundCount++;
        if (RoundManager._instance.roundCount == 0)
            RoundManager._instance.roundCount = 1;
        highScore.text = RoundManager._instance.LowestScoreRecord.ToString();
        if (RoundManager._instance.roundCount == 4)
        {
            RoundManager._instance.roundCount = 1;
            RecordHighScore();
        }
        scoreText.text = RoundManager._instance.thisHoldSum.ToString();
        roundCount.text = RoundManager._instance.roundCount.ToString();
    }

    void RecordHighScore()
    {
        if (RoundManager._instance.thisHoldSum < RoundManager._instance.LowestScoreRecord)
            RoundManager._instance.LowestScoreRecord = RoundManager._instance.thisHoldSum;
        RoundManager._instance.thisHoldSum = 0;
        highScore.text = RoundManager._instance.LowestScoreRecord.ToString();
        roundCount.text = RoundManager._instance.roundCount.ToString();
    }
    public static void EVENT(eScoreEvent evt)
    {
        try
        {
            S.Event(evt);
        }catch(System.NullReferenceException nre)
        {
            Debug.Log(nre);
        }
    }

    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLoss:
                break;
            case eScoreEvent.mine:
                currentScore = Golf.S.tableau.Count;
                UpdateScoreBoard();
                break;
            case eScoreEvent.gameEnd:
                UpdateScoreBoard();
                ShowGameResult();
                break;
        }

        switch (evt)
        {
            case eScoreEvent.gameEnd:
                currentScore = Golf.S.tableau.Count;
                RoundManager._instance.thisHoldSum += currentScore;
                break;
        }
    }

    static public int SCORE { get { return S.currentScore; } }

    public void UpdateScoreBoard()
    {
        currentScore = Golf.S.tableau.Count;
        scoreBoard.text = currentScore.ToString();
    }

    public void ShowGameResult()
    {
        Golf.S.roundResultText.text = "You Have " + currentScore.ToString() + " Cards Left in the Tableau";
        Golf.S.ShowResultsUI(true);
    }
}
