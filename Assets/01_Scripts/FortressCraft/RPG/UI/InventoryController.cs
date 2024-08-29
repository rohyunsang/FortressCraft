using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class InventoryController : MonoBehaviour
    {
        // ������ ������ � á����
        // 
        //
        public int maxSlots = 24;
        public int currentSlotCount = 0;
        public Transform[] slotTransforms;

        // ��� ������ ù ��° ������ Transform�� ��ȯ�ϴ� �Լ�
        public Transform GetNextAvailableSlot()
        {
            // currentSlotCount�� maxSlots �̸��� ��쿡�� ���� ������ ��ȯ
            if (currentSlotCount < maxSlots)
            {
                return slotTransforms[currentSlotCount];
            }
            // ��� ������ ��� ���� ��� null ��ȯ
            return null;
        }
    }


    /////////////////////////
    /////////////////////////
    ///////////////////////// ���� ������ Transform�� �޾ƿ��ϱ� ����ٰ� �������ֱ�. 
}