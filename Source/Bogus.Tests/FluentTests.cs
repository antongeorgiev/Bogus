﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Bogus.DataSets;
using NUnit.Framework;

namespace Bogus.Tests
{
    [TestFixture]
    public class Examples : SeededTest
    {
        [Test]
        public void TestAPIDesign()
        {
            //Set the randomzier seed if you wish to generate repeatable data sets.
            Randomizer.Seed = new Random(3897234);

            var fruit = new[] { "apple", "banana", "orange", "strawberry", "kiwi" };

            var orderIds = 0;
            var testOrders = new Faker<Order>()
                //Ensure all properties have rules.
                .StrictMode(true)
                //OrderId is deterministic
                .RuleFor(o => o.OrderId, f => orderIds++)
                //Pick some fruit from a basket
                .RuleFor(o => o.Item, f => f.PickRandom(fruit))
                //A random quantity from 1 to 10
                .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10));



            var userIds = 0;
            var testUsers = new Faker<User>()
                //Optional: Call for objects that have complex initialization
                .CustomInstantiator(f => new User(userIds++, f.Random.Replace("###-##-####")))

                //Basic rules using built-in generators
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))

                //Use an enum outside scope.
                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                //Use a method outside scope.
                .RuleFor(u => u.CartId, f => Guid.NewGuid())
                //Compound property with context, use the first/last name properties
                .RuleFor(u => u.FullName, (f, u) => u.FirstName + " " + u.LastName)
                //And composability of a complex collection.
                .RuleFor(u => u.Orders, f => testOrders.Generate(3).ToList())
                //After all rules are applied finish with the following action
                .FinishWith((f, u) =>
                    {
                        Console.WriteLine("User Created! Name={0}", u.FullName);
                    });

            //RunAfter
            //InvokeAfterRules
            //FinalizeWith
            //AfterRulesInvoke
            //PostProcess
            //InvokeAfterAll
            //BeforeReturn
            //FinallyInvoke
            //FinishWith
            
            var user = testUsers.Generate(3);

            user.Dump();
        }

        [Test]
        public void Without_Fluent_Syntax()
        {
            var random = new Randomizer();
            var lorem = new Lorem();
            var o = new Order()
                {
                    OrderId = random.Number(1, 100),
                    Item = lorem.Sentance(),
                    Quantity = random.Number(1, 10)
                };
            o.Dump();
        }

        [Test]
        public void With_Korean_Locale()
        {
            var lorem = new Lorem(locale: "ko");
            Console.WriteLine(lorem.Sentance(5));
        }

        [Test]
        public void Create_Context_Related_Person()
        {
            var person = new Person();

            person.Dump();
        }

        [Test]
        public void Create_an_SSN()
        {
            var ssn = new Randomizer().Replace("###-##-####");
            ssn.Dump();
        }

        [Test]
        public void get_all_locales()
        {
            var data = Database.Data.Value;

            var locales = new List<string>();
            
            foreach( var prop in data.Properties().OrderBy( p => p.Name) )
            {
                var code = prop.Name;
                var title = prop.First["title"];

                var str = string.Format("|{0,-14}|{1}", "`"+code+"`", title);
                locales.Add(str);
            }

            Console.WriteLine(string.Join("\n", locales));
        }

        public class Order
        {
            public int OrderId { get; set; }
            public string Item { get; set; }
            public int Quantity { get; set; }
        }

        public enum Gender
        {
            Male,
            Female
        }

        public class User
        {
            public User(int userId, string ssn)
            {
                this.Id = userId;
                this.SSN = ssn;
            }

            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FullName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Avatar { get; set; }
            public Guid CartId { get; set; }
            public string SSN { get; set; }
            public Gender Gender { get; set; }

            public List<Order> Orders { get; set; }
        }



    }





}
