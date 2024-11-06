using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{

    [SerializeField] List<Table> freeTables = new List<Table>(); //Serialize potrzebne tylko do debugu
    [SerializeField] List<Table> occupiedTables = new List<Table> ();
    void Awake(){
        Table[] tables = FindObjectsOfType<Table>();
        foreach (Table table in tables){
            if (table.isOccupied){
                occupiedTables.Add(table);
            }
            else{
                freeTables.Add(table);
            }
            table.tableStatusChanged += UpdateTable;
        }
    }

    public Table GetFreeTable(){
        Debug.Log(freeTables.Count);
        if(freeTables.Count>0){
            int random = Random.Range(0,freeTables.Count);
            Debug.Log("Random: " + random);
            Table chosenTable = freeTables[random];
            freeTables[random].ChangeTableStatus();
            return chosenTable;
        }
        else return null;
    }



    private void UpdateTable(Table table ,bool isOccupied){
        if(isOccupied){
            freeTables.Remove(table);
            occupiedTables.Add(table);
        }
        else{
            occupiedTables.Remove(table);
            freeTables.Add(table);
        }
    }
}
