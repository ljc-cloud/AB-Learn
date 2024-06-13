using XLua;

namespace LuaGame {
    [LuaCallCSharp]
    public class PlayerAttackStatus {


        public float GetAtk() {
            return PlayerAttack.Instance.Atk;
        }
        public float GetMatk() => PlayerAttack.Instance.Matk;
    }
}
