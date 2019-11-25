using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************************************************************
Custom class which contains a list of goals as well as accesors
This class is required in order to facilitate a list of lists appearing in the inspector
************************************************************************************************************/
[System.Serializable]
public class Goal {
    public List<GoalType> goals = new List<GoalType>();

    // Override for the count property
    public int Count {
        get {
            return goals.Count;
        }
    }

    // Override for index accessing
    public GoalType this[int index] {
        get {
            return goals[index];
        }
    }

    // Override for removing item at an index
    public void RemoveAt(int index) {
        goals.RemoveAt(index);
    }
}

/************************************************************************************************************
Custom class which contains a list of goals as well as accesors
This class is required in order to facilitate a list of lists appearing in the inspector
************************************************************************************************************/
[System.Serializable]
public class ObjectiveList {
    public List<Goal> days = new List<Goal>();

    // Overrride for the count property
    public int Count {
        get {
            return days.Count;
        }
    }

    // Override for index accessing
    public Goal this[int index] {
        get {
            return days[index];
        }
    }
}
