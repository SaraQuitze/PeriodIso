using UnityEngine;

public enum FloorType
{
    Reception,
    Cafeteria,
    Printing,
    HR,
    Accounting,
    Offices
}

public class FloorManager : MonoBehaviour
{
    public FloorType currentFloor;

    public void ChangeFloor(FloorType newFloor)
    {
        currentFloor = newFloor;
        // Lógica de cambio de escena/visual
    }
}