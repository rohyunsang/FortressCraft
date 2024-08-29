using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class InventoryController : MonoBehaviour
    {
        // 아이템 슬롯이 몇개 찼는지
        // 
        //
        public int maxSlots = 24;
        public int currentSlotCount = 0;
        public Transform[] slotTransforms;

        // 사용 가능한 첫 번째 슬롯의 Transform을 반환하는 함수
        public Transform GetNextAvailableSlot()
        {
            // currentSlotCount가 maxSlots 미만인 경우에만 다음 슬롯을 반환
            if (currentSlotCount < maxSlots)
            {
                return slotTransforms[currentSlotCount];
            }
            // 모든 슬롯이 사용 중인 경우 null 반환
            return null;
        }
    }


    /////////////////////////
    /////////////////////////
    ///////////////////////// 이제 슬롯의 Transform을 받아오니까 여기다가 생성해주기. 
}