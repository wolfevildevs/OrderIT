using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEventSO gameEvent;
    [SerializeField] private UnityEvent onEventTriggered;

    private void OnEnable() => gameEvent.RegisterListener(this);
    private void OnDisable() => gameEvent.UnregisterListener(this);

    public void OnEventRaised() => onEventTriggered.Invoke();
}