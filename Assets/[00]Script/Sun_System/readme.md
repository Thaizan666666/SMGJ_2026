## how to use Coversystem
หลักๆ จะใช้แค่เอา ```CoverSystem.cs``` โดยเอาไปใส่ใน 
แล้วใส่ Collider 2D แบบใดก็ได้ 

### method สำคัญๆ
- `private bool IsPlayerFullyInside(Collider2D playerCollider)`
    - ใช้ Check ว่าผู้เล่นเข้าไปอยู่ใน Cover ทั้งตัวหรือไม่
    - return type : boolen
- `private float GetOverlapPercentage(Collider2D playerCollider)`
    - ใช้บอก % ของตัวผู้เล่นว่าเข้ามาอยู่ใน Cover กี่ %
    - return type : float

## การ Setting โดยใช้ CoverSystemSetting.cs เป็น ScriptableObject
 ที่ใช้สำหรับการปรับแต่งค่าพื้นฐานของระบบ Cover และ Sun System โดยมีรายละเอียด Field ดังนี้:
 ### 1. Target Name
 ใช้สำหรับระบุตัวตนของ Player ในฉากPlayer 
- Tag: ชื่อ Tag ของ Object ที่เป็นผู้เล่น (เริ่มต้นคือ "Player") เพื่อให้ระบบค้นหา Object ได้ถูกต้องPlayer 
- Layer: เลเยอร์ที่ใช้ระบุตัวตนของ Player (LayerMask) เพื่อจำกัดขอบเขตการตรวจสอบทางฟิสิกส์
 ### 2. Overlap Sampling of 
 Playerส่วนนี้ใช้กำหนดความละเอียดในการสุ่มจุดตรวจสอบ (Sampling) บนตัวละคร เพื่อเช็คว่าผู้เล่นอยู่ในที่ร่มหรือกลางแดดSample 
- Resolution: จำนวนจุดสุ่มต่อแกน (Grid Resolution)มีช่วงค่าอยู่ที่ 2 ถึง 6ตัวอย่าง: หากตั้งค่าเป็น 3 ระบบจะสร้างจุดสุ่มทั้งหมด $3 \times 3 = 9$ จุด รอบตัวผู้เล่น 

`หมายเหตุ: ค่าที่สูงขึ้นจะให้ความแม่นยำที่มากขึ้น แต่จะแลกมาด้วยการใช้งาน CPU ที่สูงขึ้นตามไปด้วย`
 ### 3. Hot Gauge System Settingค่าตัวแปรสำหรับการคำนวณเกจความร้อน (Hot Gauge)Hot 
- Gauge Increase Rate: อัตราการเพิ่มขึ้นของเกจความร้อนต่อวินาที เมื่อผู้เล่นสัมผัสแสงแดด
- Hot Gauge Decrease Rate: อัตราการลดลงของเกจความร้อนต่อวินาที เมื่อผู้เล่นอยู่ในที่ร่ม (Cover)

 ### 4. Debug Partส่วนช่วยในการตรวจสอบการทำงาน (Visualization)
 - Debug Color: สีที่ใช้แสดงผลในหน้าต่าง Scene หรือ Gizmos เพื่อช่วยให้ Developer มองเห็นขอบเขตของ Collider

