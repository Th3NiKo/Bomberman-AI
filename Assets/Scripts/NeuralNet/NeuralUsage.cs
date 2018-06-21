using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
public class NeuralUsage : MonoBehaviour {

	NeuralNetwork net;
	int playerIndex = 30;
	int index = 2;
	GameManager gameManager;
	float turnTime;
	float timer = 0;
	int [,] map;
	float [] values;
	Vector2 lastPosition;
	public bool Train = false;
	int lastHp;
	void Start () {
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		turnTime = gameManager.GetTurnTime();
		net = new NeuralNetwork(new int[] { gameManager.rowsCount * gameManager.columnsCount, 50, 50, 5 }); 
		map = gameManager.GetMap();
		net.LoadWeights();
		values = new float [5];
//		lastPosition = new Vector2(gameManager.players[index].x,gameManager.players[index].y);
//		lastHp = gameManager.players[index].health;
	}
	
	
	void Update () {
		if(Train == true){
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

		if(timer > turnTime){
			
			map = gameManager.GetMap();
			int [] mapa1D = map.Cast<int>().ToArray();
			float [] FinalMap = new float[mapa1D.Length];
			for(int i = 0;i < mapa1D.Length; i++){
				switch(mapa1D[i]){
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
			}
			if(Train == false){
			values = net.FeedForward(FinalMap);


			float max = values[0];
			int maxIndex = 0;
			for(int i = 1;i < values.Length; i++){
				if(max < values[i]){
					max = values[i];
					maxIndex = i;
				}
			}
			Debug.Log(maxIndex);

			switch(maxIndex){
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
			
			if(Train == true){
			net.FeedForward(FinalMap);
			float [] values = {0,0,0,0,0};
			switch(gameManager.players[2].lastAction){
				case Action.MoveForward:
				values[0]= 1;
				break;
				case Action.RotateClockwise:
				values[1]= 1;
				break;
				case Action.RotateCounterClockwise:
				values[2]= 1;
				break;
				case Action.PlaceBomb:
				values[3]= 1;
				break;
				case Action.Wait:
				values[4]= 1;
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
			if(gameManager.GameFinished() != -1){
			
				net.SaveWeights();
				
				 Scene loadedLevel = SceneManager.GetActiveScene ();
  				 SceneManager.LoadScene (loadedLevel.buildIndex);

			} 
			lastPosition = new Vector2(gameManager.players[index].x,gameManager.players[index].y);
			lastHp = gameManager.players[index].health;
			}
		}
		timer += Time.deltaTime;
	}
}
