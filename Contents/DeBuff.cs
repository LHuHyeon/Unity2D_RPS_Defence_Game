using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeBuff
{
    public InstantBuffData  _deBuffData;        // 정보
    public EnemyStat        _enemyStat;         // 몬스터 정보
    
    public float    _elapsedTime = 0;           // 남은 시간

    private bool    _isDebuffActive = false;    // 디버프 진행 여부

    // 디버프 등록
    public bool ApplyDebuff(InstantBuffData deBuffData)
    {
        // 쿨타임 초기화
        _elapsedTime = deBuffData.time;

        // 이미 진행 중이라면
        if (_isDebuffActive == true)
        {
            // 들어온 값이 현재 값보다 높으면 넘어갈 수 있음!
            if (deBuffData.value <= _deBuffData.value)
                return false;
        }

        _deBuffData = deBuffData;

        // 확률 적용
        if (_deBuffData.parcentage <= Random.Range(0, 101))
            return false;
        
        // 값 적용
        switch (_deBuffData.buffType)
        {
            case Define.DeBuffType.DefenceDecrease:    // 방어력 감소
                _enemyStat.Defence = _enemyStat.MaxDefence - Mathf.RoundToInt(_enemyStat.MaxDefence * (_deBuffData.value * 0.01f));
                break;
            case Define.DeBuffType.Slow:               // 이동속도 감소
                _enemyStat.MoveSpeed = _enemyStat.MaxMoveSpeed - _enemyStat.MaxMoveSpeed * (_deBuffData.value * 0.01f);
                break;
            case Define.DeBuffType.Stun:               // 기절/경직
                _enemyStat.MoveSpeed = 0;
                break;
        }

        _isDebuffActive = true;

        return true;
    }

    // 디버프 종료
    public void EndDebuff()
    {
        switch (_deBuffData.buffType)
        {
            case Define.DeBuffType.DefenceDecrease:    // 방어력 감소
                _enemyStat.Defence = _enemyStat.MaxDefence;
                break;
            case Define.DeBuffType.Slow:               // 이동속도 감소
            case Define.DeBuffType.Stun:               // 기절/경직
                _enemyStat.MoveSpeed = _enemyStat.MaxMoveSpeed;
                break;
        }

        _isDebuffActive = false;
    }
}
