using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents: MonoBehaviour
{
    public UnityEvent OnGameStarted { get; } = new();
    public UnityEvent OnGameFinished { get; } = new();
    public UnityEvent OnMoveMade { get; } = new();
    public UnityEvent OnReset { get; } = new();
}