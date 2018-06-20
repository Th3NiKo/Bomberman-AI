using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class QUsage2 : MonoBehaviour {
	
	int playerIndex = 10;
	int index = 0;
	GameManager gameManager;
	float turnTime;
	float timer = 0;
	int [,] map;
	int [,] lastMap;
	float [] values;
	float turn = 0;
	Vector2 lastPosition;
	public bool Train = false;
	int lastHp;
	int population = 20;
	float reward = 0.0f;
	QLearnerScript Ql;
	
	bool weLost = false;
	int gameFinished = -1;

	void Start () {
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		turnTime = gameManager.GetTurnTime();
		map = gameManager.GetMap();
		Ql = new QLearnerScript(5);
		lastMap = map;
		lastHp = gameManager.players[index].health;
	}

	static int[] MultiToSingle(int[,] array)
	{
		int index = 0;
		int width = array.GetLength(0);
		int height = array.GetLength(1);
		int[] single = new int[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				single[index] = array[x, y];
				index++;
			}
		}
		return single;
	}
	
	
	void Update () {
		if(timer > turnTime){
			
			map = gameManager.GetMap();
			float [] mapa = new float [(gameManager.rowsCount-2) * (gameManager.columnsCount-2) * 4 + 1];
			int [,] mapaPrzerobiona = new int [gameManager.rowsCount-2, gameManager.columnsCount-2];
			for(int i = 1; i < gameManager.rowsCount - 1; i++){
				for(int j = 1; j < gameManager.columnsCount - 1; j++){
					mapaPrzerobiona[i-1,j-1] = map[i,j];
				}
			}
			
			int counter = 0;
			for(int i = 0; i < gameManager.rowsCount -2; i++){
				for(int j = 0; j < gameManager.columnsCount -2; j++){
					switch(mapaPrzerobiona[i,j]){
					case 0:
					mapa[counter] = 1;
					mapa[counter+1] = 0;
					mapa[counter+2] = 0;
					mapa[counter+3] = 0;
					counter+=4;
					break;
					case 6:
					mapa[counter] = 1;
					mapa[counter+1] = 0;
					mapa[counter+2] = 0;
					mapa[counter+3] = 0;
					counter++;
					break;
					case 8:
					mapa[counter] = 0;
					mapa[counter+1] = 0;
					mapa[counter+2] = 0;
					mapa[counter+3] = 0;
					break;
					case 9:
					mapa[counter] = -1;
					mapa[counter+1] = 0;
					mapa[counter+2] = 0;
					mapa[counter+3] = 0;
					break;
					case 1:
					mapa[counter] = -1;
					mapa[counter+1] = 0;
					mapa[counter+2] = 0;
					if(gameManager.whosBombItIs(i,j) == index){
						mapa[counter+3] = 1.0f;
					} else {
						mapa[counter+3] = -1.0f;
					}
					
					counter++;
					break;
					case 2:
					mapa[counter] = -1;
					mapa[counter+1] = 0;
					mapa[counter+2] = 0;
					if(gameManager.whosBombItIs(i,j) == index){
						mapa[counter+3] = 0.5f;
					} else {
						mapa[counter+3] = -0.5f;
					}
					counter++;
					break;
					case 3:
					mapa[counter] = -1;
					mapa[counter+1] = 0;
					mapa[counter+2] = 0;
					if(gameManager.whosBombItIs(i,j) == index){
						mapa[counter+3] = 0.34f;
					} else {
						mapa[counter+3] = -0.34f;
					}
					counter++;
					break;
					case 4:
					mapa[counter] = -1;
					mapa[counter+1] = 0;
					mapa[counter+2] = 0;
					if(gameManager.whosBombItIs(i,j) == index){
						mapa[counter+3] = 0.25f;
					} else {
						mapa[counter+3] = -0.25f;
					}
					counter++;
					break;
					case 10:
					mapa[counter] = -1;
					mapa[counter+1] = 0;
					mapa[counter+2] = 1;
					mapa[counter+3] = 0;
					counter++;
					break;
					case 20:
					mapa[counter] = -1;
					mapa[counter+1] = 1;
					mapa[counter+2] = 0;
					mapa[counter+3] = 0;
					counter++;
					break;
					case 30:
					mapa[counter] = -1;
					mapa[counter+1] = 1;
					mapa[counter+2] = 0;
					mapa[counter+3] = 0;
					counter++;
					break;
				}
				}
			}
			switch(gameManager.players[index].Orientation){
				case Direction.DOWN:
				mapa[counter] = 0f;
				break;

				case Direction.UP:
				mapa[counter] = 0.75f;
				break;

				case Direction.LEFT:
				mapa[counter] = 0.5f;
				break;

				case Direction.RIGHT:
				mapa[counter] = 1.0f;
				break;
			}
			
			// Learning 
			
			if(Train == true){
			
			int action = Ql.main(mapa, reward);
			gameManager.ShowErrors(action);
			reward = 0.0f;
			switch(action){
				case 0:
				if(gameManager.CanMoveForward(playerIndex)){
					reward -= 1;
				} else {
					reward -= 2;
				}
				gameManager.MoveForward(playerIndex);
				
				break;
				case 1:
				gameManager.RotateClockwise(playerIndex);
				reward -= 1f;
				
				break;
				case 2:
				gameManager.RotateCounterClockwise(playerIndex);
				reward -= 1f;
				
				break;
				case 3:
				if(gameManager.CanMoveForward(playerIndex)){
					reward -= 1;
				} else {
					reward -= 2;
				}
				gameManager.PlaceBomb(playerIndex);
				
				break;
				case 4:
					reward -= 1;
				break;
			}

			if(gameManager.isBlowingNearPlayer(playerIndex)){
				reward -= 5;
			}
			

			if(gameManager.players[index].killedPlayer == 20 || gameManager.players[index].killedPlayer == 30){
				reward += 100;
				gameManager.players[index].killedPlayer = -1;
			}
			if(gameManager.players[index].killedWall){
				reward += 30;
				gameManager.players[index].killedWall = false;
			}

			
			
			if(gameManager.players[index].health <= 0){
				weLost = true;
				reward -= 300;
			}
			timer = 0;

			if(gameFinished != -1 || Input.GetKey(KeyCode.R))
			{
				Debug.Log(gameManager.GameFinished());
				//Ql.SaveWeights();
				Scene loadedLevel = SceneManager.GetActiveScene ();
  				SceneManager.LoadScene (loadedLevel.buildIndex);
			}

			if(gameManager.GameFinished() != -1 ){
				gameFinished = gameManager.GameFinished();
				if(gameFinished == playerIndex){
					//No reward now
				} else {
					reward -= 300;
				}
				
			} 
			

			}
			lastMap = map;
		}
		timer += Time.deltaTime;
	}
}