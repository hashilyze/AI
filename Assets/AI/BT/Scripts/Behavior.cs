using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BT {
    public enum EState { Undefined, Failure, Success, Running, Aborted }
    
    public abstract class Behavior : ScriptableObject {
        public virtual void Initailize() { }
        public virtual void Terminate() { }
        public abstract EState Update();
        
        public EState Tick(EState state) {
            if (state != EState.Running) Initailize();
            state = Update();
            if (state != EState.Running) Terminate();
            return state;
        }
    }
}