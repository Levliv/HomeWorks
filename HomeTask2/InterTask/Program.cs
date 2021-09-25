using System;

namespace InterTask
{
    class Program
    {

        interface IItem
        {
            string GetInfo();
            string GetItemType();
        }
        abstract class Weapon : IItem
        {
            public abstract void Fire();

            public string GetInfo()
            {
                return GetType().Name;
            }
            public string GetItemType()
            {
                return "Weapon";
            }
        }
        class Gun : Weapon
        {
            public override void Fire()
            {
                Console.WriteLine("Пиу!");
            }
        }
        class MachineGun: Weapon
        {
            public override void Fire()
            {
                Console.WriteLine("Паф");
            }
        }
        class Person
        {
            public void TakeFire(Weapon weapon)
            {
                weapon.Fire();
            }
        }
        static void Main(string[] args)
        {
            var person = new Person();
            person.TakeFire(new Gun());
            person.TakeFire(new MachineGun());
            var gun1 = new Gun();
            var str = gun1.GetItemType();
            var str2 = gun1.GetInfo();
            Console.WriteLine("This is " + str);
            Console.WriteLine("This is " + str2);

        }
    }
}
