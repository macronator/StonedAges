using System.Collections.Generic;

namespace Engine;

public class StateSystem
{
    private Dictionary<string, IGameObject> _stateStore = new Dictionary<string, IGameObject>();

    private IGameObject _currentState;

    public void Update(double elapsedTime)
    {
        if (_currentState != null)
        {
            _currentState.Update(elapsedTime);
        }
    }

    public void Render(double elapsedTime)
    {
        if (_currentState != null)
        {
            _currentState.Render(elapsedTime);
        }
    }

    public void AddState(string stateId, IGameObject state)
    {
        _stateStore.Add(stateId, state);
    }

    public void ChangeState(string stateId)
    {
        _currentState = _stateStore[stateId];
    }

    public bool Exists(string stateId)
    {
        return _stateStore.ContainsKey(stateId);
    }
}
