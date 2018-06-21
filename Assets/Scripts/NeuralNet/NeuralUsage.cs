using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NeuralUsage : MonoBehaviour
{
    private GameManager gameManager;
    private readonly int index = 2;
    private int lastHp;
    private Vector2 lastPosition;
    private int[,] map;

    private NeuralNetwork net;
    private readonly int playerIndex = 30;
    private float timer;
    public bool Train = false;
    private float turnTime;
    private float[] values;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        turnTime = gameManager.GetTurnTime();
        net = new NeuralNetwork(new[] {gameManager.rowsCount * gameManager.columnsCount, 50, 50, 5});
        map = gameManager.GetMap();
        net.LoadWeights();
        values = new float [5];

//		lastPosition = new Vector2(gameManager.players[index].x,gameManager.players[index].y);
//		lastHp = gameManager.players[index].health;
    }


    private void Update()
    {
        if (Train)
        {
            /* 
        int x = Random.Range(0,10);
        if(x >= 0 && x < 9){
                if(gameManager.CanMoveForward(playerIndex))
                {
                    gameManager.MoveForward(playerIndex);
                    
                } else {
                    x = Random.Range(0,1);
                    if(x == 0){
                        gameManager.RotateCounterClockwise(playerIndex);
                    } else if(x == 1){
                        gameManager.RotateClockwise(playerIndex);
                    }
                }
            
            } else {
                if(gameManager.CanMoveForward(playerIndex))
                {
                    gameManager.PlaceBomb(playerIndex);
                } else {
                    x = Random.Range(0,1);
                    if(x == 0){
                        gameManager.RotateCounterClockwise(playerIndex);
                    } else if(x ==1){
                        gameManager.RotateClockwise(playerIndex);
                    }
                }
            }
        }*/
        }

        if (timer > turnTime)
        {
            map = gameManager.GetMap();
            var mapa1D = map.Cast<int>().ToArray();
            var FinalMap = new float[mapa1D.Length];
            for (var i = 0; i < mapa1D.Length; i++)
                switch (mapa1D[i])
                {
                    case 0:
                        FinalMap[i] = 0;
                        break;
                    case 6:
                        FinalMap[i] = 0.1f;
                        break;
                    case 8:
                        FinalMap[i] = 0.2f;
                        break;
                    case 9:
                        FinalMap[i] = 0.3f;
                        break;
                    case 1:
                        FinalMap[i] = 0.5f;
                        break;
                    case 2:
                        FinalMap[i] = 0.55f;
                        break;
                    case 3:
                        FinalMap[i] = 0.60f;
                        break;
                    case 4:
                        FinalMap[i] = 0.65f;
                        break;
                    case 10:
                        FinalMap[i] = 1.0f;
                        break;
                    case 20:
                        FinalMap[i] = 0.9f;
                        break;
                    case 30:
                        FinalMap[i] = 0.8f;
                        break;
                    default:
                        Debug.Log("Something else");
                        break;
                }
            if (Train == false)
            {
                values = net.FeedForward(FinalMap);


                var max = values[0];
                var maxIndex = 0;
                for (var i = 1; i < values.Length; i++)
                    if (max < values[i])
                    {
                        max = values[i];
                        maxIndex = i;
                    }

                Debug.Log(maxIndex);

                switch (maxIndex)
                {
                    case 0:
                        gameManager.MoveForward(playerIndex);
                        break;
                    case 1:
                        gameManager.RotateClockwise(playerIndex);
                        break;
                    case 2:
                        gameManager.RotateCounterClockwise(playerIndex);
                        break;
                    case 3:
                        gameManager.PlaceBomb(playerIndex);
                        break;
                }

                gameManager.ShowError(maxIndex);
            }
            // Learning 

            if (Train)
            {
                net.FeedForward(FinalMap);
                float[] values = {0, 0, 0, 0, 0};
                switch (gameManager.players[2].lastAction)
                {
                    case Action.MoveForward:
                        values[0] = 1;
                        break;
                    case Action.RotateClockwise:
                        values[1] = 1;
                        break;
                    case Action.RotateCounterClockwise:
                        values[2] = 1;
                        break;
                    case Action.PlaceBomb:
                        values[3] = 1;
                        break;
                    case Action.Wait:
                        values[4] = 1;
                        break;
                }

                gameManager.players[2].lastAction = Action.Wait;
                net.BackProp(values);

                /* 
                if(gameManager.players[index].health <= 0){
                    Debug.Log(gameManager.GameFinished());
                    net.SaveWeights();
                    Scene loadedLevel = SceneManager.GetActiveScene ();
                       SceneManager.LoadScene (loadedLevel.buildIndex);
                }*/
                timer = 0;
                if (gameManager.GameFinished() != -1)
                {
                    net.SaveWeights();

                    var loadedLevel = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(loadedLevel.buildIndex);
                }

                lastPosition = new Vector2(gameManager.players[index].x, gameManager.players[index].y);
                lastHp = gameManager.players[index].health;
            }
        }

        timer += Time.deltaTime;
    }
}