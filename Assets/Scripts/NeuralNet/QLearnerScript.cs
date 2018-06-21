using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Globalization;


	public class QLearnerScript
	{
		
		private List<float[]> QtStates;
		private List<float[]> QtActions;

		private int possibleActions;

		private float[] initialState;
		private int initialActionIndex;
		private float[] outcomeState;
		private float outcomeActionValue;

		private float lr;
		private float y;

		private float SimInterval;

		private bool firstIteration;
		System.Random random = new System.Random();

		public QLearnerScript(int possActs)
		{
			QtStates = new List<float[]>();
			QtActions = new List<float[]>();
			possibleActions = possActs;

			lr = .8f;
			y = .95f;

			firstIteration = true;
		}

		

		public int getQtableCount()
		{
			return QtStates.Count;
		}

		public int main(float[] curState, float reward)
		{
			Debug.Log("Actual reward: " + reward.ToString());
			step2(curState, reward);

			initialState = curState;

			firstIteration = false;

			int actionIndex = random.Next(0, possibleActions);

			bool exists = false;
			if(QtStates.Count > 0)
			{
				for(int i = 0; i < QtStates.Count; i++)
				{
					float[] state = QtStates.ElementAt(i);
					float[] actions = QtActions.ElementAt(i);

					int counter = 0;
					for(int j = 0; j < initialState.Length; j++){
						if(state[j] == initialState[j]){
							counter++;
						}
					}
					if(counter >= initialState.Length - 1){
						exists = true;
						float random = UnityEngine.Random.Range(0.0f, 1.0f);
						if(random < 0.75f){
							initialActionIndex = Array.IndexOf(actions, MaxFloat(actions));
						} else {
							int pickRandom = UnityEngine.Random.Range(0,5);
							return pickRandom;
						}

						return initialActionIndex;
					}
				}
			}

			if(!exists)
			{
				float[] actionVals = new float[possibleActions];
				for (int i = 0; i < possibleActions; i++)
				{
					actionVals[i] = 0f;
				}
				QtStates.Add(initialState);
				QtActions.Add(actionVals);
				

			}

			initialActionIndex = actionIndex;
			return initialActionIndex;
		}

		public void step2(float[] outcmState, float reward)
		{
			if(!firstIteration)
			{
				outcomeState = outcmState;

				bool exists = false;
				for(int i = 0; i < QtStates.Count; i++)
				{
					float[] state = QtStates.ElementAt(i);
					float[] actions = QtActions.ElementAt(i);
					int counter = 0;
					for(int j = 0; j < outcomeState.Length; j++){
						if(state[j] == outcomeState[j]){
							counter++;
						}
					}
					if(counter >= outcomeState.Length - 1){
						exists = true;
						float random = UnityEngine.Random.Range(0.0f, 1.0f);
						if(random < 0.75f){

							outcomeActionValue = MaxFloat(actions);
						} else {
							int pickRandom = UnityEngine.Random.Range(0,5);
							outcomeActionValue = pickRandom;
						}
						
					}

				}

				for(int i = 0; i < QtStates.Count; i++)
				{
					float[] state = QtStates.ElementAt(i);
					float[] actions = QtActions.ElementAt(i);

					int counter = 0;
					for(int j = 0; j < initialState.Length; j++){
						if(state[j] == initialState[j]){
							counter++;
						}
					}
					if(counter >= initialState.Length - 1){
						if(exists)
						{
							actions[initialActionIndex] = (actions[initialActionIndex] + lr * (reward + y * outcomeActionValue - actions[initialActionIndex]));
						}

						if(!exists)
						{
							actions[initialActionIndex] = (actions[initialActionIndex] + lr * (reward + y * 0f - actions[initialActionIndex]));
						}
					}
					
				}
			} else {
				LoadWeights();
			}
		}

		private float MaxFloat(float[] numbers)
        {
            float m = numbers[0];

            for (int i = 0; i < numbers.Length; i++)
                if (m < numbers[i])
                {
                    m = numbers[i];
                }

            return m;
        }

		public void SaveWeights(){
			StreamWriter sw = new StreamWriter(@"C:\Users\Kysko\Documents\Nowy folder (2)\Bomberman-AI\wagi.txt");
			sw.WriteLine(QtStates.Count);
			for(int i = 0; i < QtStates.Count; i++){
				sw.WriteLine(QtStates[i].Length);
				for(int j = 0; j < QtStates[i].Length; j++){
					sw.WriteLine(QtStates[i][j]);
				}
			}
			sw.WriteLine(QtActions.Count);
			for(int i = 0; i < QtActions.Count; i++){
				sw.WriteLine(QtActions[i].Length);
				for(int j = 0; j < QtActions[i].Length; j++){
					sw.WriteLine(QtActions[i][j]);
				}
			}
			sw.Dispose();
		}


		public void LoadWeights(){
			StreamReader sr = new StreamReader(@"C:\Users\Kysko\Documents\Nowy folder (2)\Bomberman-AI\wagi.txt");
			int QCount = int.Parse(sr.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
			for(int i = 0; i < QCount; i++){
				int Qtemp = int.Parse(sr.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
				float[] temporary = new float[Qtemp];
				for(int j = 0; j < Qtemp; j++){
					if(sr.Peek() >= 0)
					temporary[j] = float.Parse(sr.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
				}
				QtStates.Add(temporary);
			}
			QCount = int.Parse(sr.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
			for(int i = 0; i < QCount; i++){
				int Qtemp = int.Parse(sr.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
				float[] temporary = new float[Qtemp];
				for(int j = 0; j < Qtemp; j++){
					if(sr.Peek() >= 0)
					temporary[j] = float.Parse(sr.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
				}
				QtActions.Add(temporary);
			}
			 sr.Dispose();

		}
	}
