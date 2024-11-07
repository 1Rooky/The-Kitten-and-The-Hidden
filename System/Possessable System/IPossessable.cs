using UnityEngine;

public interface IPossessable
{
    GameObject GhostPlayer { get; set; }

    void Possess();

    void Unpossess();
}