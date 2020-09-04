//
//  CActionRegistryTest.cs
//
//  Lunar Unity Mobile Console
//  https://github.com/SpaceMadness/lunar-unity-console
//
//  Copyright 2015-2020 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//


using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using LunarConsolePlugin;
using LunarConsolePluginInternal;

namespace Actions
{
    [TestFixture]
    public class CRegistryTest : TestFixtureBase, ICRegistryDelegate
    {
        private CRegistry m_registry;

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            m_registry = new CRegistry();
            m_registry.registryDelegate = this;
        }

        #endregion

        #region Target Registration

        [Test]
        public void TestRegisterTargets()
        {
            var target = new DummyTarget();
            var disposable = m_registry.Register(target);

            var actual = m_registry.actions.ToList();
            var expected = new[]
            {
                new CAction(0, "Public Action", (Action) target.PublicAction, requiresConfirmation: false),
                new CAction(1, "Private Action", (Action) target.privateAction, requiresConfirmation: false),
                new CAction(2, "Public Static Action", (Action) DummyTarget.PublicStaticAction, requiresConfirmation: false),
                new CAction(3, "Private Static Action", (Action) DummyTarget.privateStaticAction, requiresConfirmation: false)
            };
            Assert.AreEqual(expected, actual);

            disposable.Dispose();
            Assert.AreEqual(0, m_registry.actions.Count);
        }

        #endregion

        #region Register actions

        [Test]
        public void TestRegisterActions()
        {
            RegisterAction("a1", Del1);
            RegisterAction("a2", Del2);
            RegisterAction("a3", Del3);

            AssertActions(
                new CActionInfo("a1", Del1),
                new CActionInfo("a2", Del2),
                new CActionInfo("a3", Del3)
            );
        }

        [Test]
        public void TestRegisterMultipleActionsWithSameName()
        {
            RegisterAction("a1", Del1);
            RegisterAction("a2", Del2);
            RegisterAction("a3", Del3);
            RegisterAction("a3", Del4);

            AssertActions(
                new CActionInfo("a1", Del1),
                new CActionInfo("a2", Del2),
                new CActionInfo("a3", Del4)
            );
        }

        #endregion

        #region Unregister actions

        [Test]
        public void TestUnregisterActionByName()
        {
            RegisterAction("a1", Del1);
            RegisterAction("a2", Del2);
            RegisterAction("a3", Del3);

            UnregisterAction("a2");

            AssertActions(
                new CActionInfo("a1", Del1),
                new CActionInfo("a3", Del3)
            );

            UnregisterAction("a1");

            AssertActions(
                new CActionInfo("a3", Del3)
            );

            UnregisterAction("a3");
            AssertActions();

            UnregisterAction("a1");
            UnregisterAction("a2");
            UnregisterAction("a3");
            AssertActions();
        }

        [Test]
        public void TestUnregisterActionByDelegate()
        {
            RegisterAction("a1", Del1);
            RegisterAction("a2", Del2);
            RegisterAction("a3", Del3);

            UnregisterAction(Del2);

            AssertActions(
                new CActionInfo("a1", Del1),
                new CActionInfo("a3", Del3)
            );

            UnregisterAction(Del1);

            AssertActions(
                new CActionInfo("a3", Del3)
            );

            UnregisterAction(Del3);
            AssertActions();

            UnregisterAction(Del1);
            UnregisterAction(Del2);
            UnregisterAction(Del3);

            AssertActions();
        }

        [Test]
        public void TestUnregisterActionByTarget()
        {
            DelClass1 delClass1 = new DelClass1();
            DelClass2 delClass2 = new DelClass2();
            DelClass3 delClass3 = new DelClass3();

            Action del1 = delClass1.Del1;
            Action del2 = delClass2.Del2;
            Action del3 = delClass3.Del3;

            RegisterAction("a1", del1);
            RegisterAction("a2", del2);
            RegisterAction("a3", del3);

            UnregisterAllActions(delClass2);

            AssertActions(
                new CActionInfo("a1", del1),
                new CActionInfo("a3", del3)
            );

            UnregisterAllActions(delClass1);

            AssertActions(
                new CActionInfo("a3", del3)
            );

            UnregisterAllActions(delClass3);

            AssertActions();

            UnregisterAllActions(delClass3);

            AssertActions();
        }

        #endregion

        #region Lookup actions

        [Test]
        public void TestLookupAction()
        {
            RegisterAction("A", Del1);
            RegisterAction("B", Del1);
            RegisterAction("C", Del1);
            RegisterAction("D", Del1);

            var a = new CAction(0, "A", (Action) Del1, false);
            var b = new CAction(1, "B", (Action) Del1, false);
            var c = new CAction(2, "C", (Action) Del1, false);
            var d = new CAction(3, "D", (Action) Del1, false);
            
            CAction[] actions = {a, b, c, d};
            for (int i = 0; i < actions.Length - 1; ++i)
            {
                for (int j = i + 1; j < actions.Length; ++j)
                {
                    Assert.AreNotEqual(actions[i].Id, actions[j].Id);
                }
            }

            Assert.AreEqual(a, FindAction(a.Id));
            Assert.AreEqual(b, FindAction(b.Id));
            Assert.AreEqual(c, FindAction(c.Id));
            Assert.AreEqual(d, FindAction(d.Id));

            UnregisterAction(a.Id);
            Assert.IsNull(FindAction(a.Id));
            Assert.AreEqual(b, FindAction(b.Id));
            Assert.AreEqual(c, FindAction(c.Id));
            Assert.AreEqual(d, FindAction(d.Id));

            UnregisterAction(b.Id);
            Assert.IsNull(FindAction(a.Id));
            Assert.IsNull(FindAction(b.Id));
            Assert.AreEqual(c, FindAction(c.Id));
            Assert.AreEqual(d, FindAction(d.Id));

            UnregisterAction(c.Id);
            Assert.IsNull(FindAction(a.Id));
            Assert.IsNull(FindAction(b.Id));
            Assert.IsNull(FindAction(c.Id));
            Assert.AreEqual(d, FindAction(d.Id));

            UnregisterAction(d.Id);
            Assert.IsNull(FindAction(a.Id));
            Assert.IsNull(FindAction(b.Id));
            Assert.IsNull(FindAction(c.Id));
            Assert.IsNull(FindAction(d.Id));
        }

        #endregion

        #region Delegate notifications

        [Test]
        public void TestDelegateNotifications()
        {
            RegisterAction("a1", Del1);
            AssertResult("added: a1");

            RegisterAction("b1", Del1);
            AssertResult("added: b1");

            RegisterAction("c1", Del1);
            AssertResult("added: c1");

            RegisterAction("d1", Del1);
            AssertResult("added: d1");
        }

        #endregion

        #region ICRegistryDelegate implementation

        public void OnActionRegistered(CRegistry registry, CAction action)
        {
            AddResult("added: " + action.Name);
        }

        public void OnActionChanged(CRegistry registry, CAction action)
        {
            AddResult("changed: " + action.Name);
        }

        public void OnActionUnregistered(CRegistry registry, CAction action)
        {
            AddResult("removed: " + action.Name);
        }

        public void OnActionsCleared(CRegistry registry)
        {
            AddResult("cleared");
        }

        public void OnVariableRegistered(CRegistry registry, CVar cvar)
        {
        }

        public void OnVariableUpdated(CRegistry registry, CVar cvar)
        {
        }

        public void OnVariableUnregistered(CRegistry registry, CVar cvar)
        {
        }

        #endregion

        #region Helpers

        private CAction FindAction(int id)
        {
            return m_registry.FindAction(id);
        }

        private IDisposable RegisterAction(string name, Action actionDelegate)
        {
            return m_registry.RegisterAction(name, actionDelegate, requiresConfirmation: false);
        }

        private void UnregisterAction(string name)
        {
            m_registry.UnregisterAction(name);
        }

        private void UnregisterAction(int id)
        {
            m_registry.UnregisterAction(id);
        }

        private void UnregisterAction(Action actionDelegate)
        {
            m_registry.UnregisterAction(actionDelegate);
        }

        private void UnregisterAllActions(object target)
        {
            m_registry.UnregisterAll(target);
        }

        private void AssertActions(params CActionInfo[] expected)
        {
            int index = 0;
            foreach (var action in m_registry.actions)
            {
                Assert.AreEqual(expected[index].name, action.Name);
                Assert.AreEqual(expected[index].actionDelegate, action.ActionDelegate);
                ++index;
            }
        }

        #endregion

        #region Action delegates

        void Del1()
        {
        }

        void Del2()
        {
        }

        void Del3()
        {
        }

        void Del4()
        {
        }

        #endregion

        #region Action classes

        class DelClass1
        {
            public void Del1()
            {
            }

            public void Del2()
            {
            }

            public void Del3()
            {
            }
        }

        class DelClass2
        {
            public void Del1()
            {
            }

            public void Del2()
            {
            }

            public void Del3()
            {
            }
        }

        class DelClass3
        {
            public void Del1()
            {
            }

            public void Del2()
            {
            }

            public void Del3()
            {
            }
        }

        #endregion
    }

    internal class DummyTarget
    {
        [ConsoleAction]
        public void PublicAction()
        {
        }

        [ConsoleAction]
        private void PrivateAction()
        {
        }

        [ConsoleAction]
        public static void PublicStaticAction()
        {
        }

        [ConsoleAction]
        private static void PrivateStaticAction()
        {
        }

        [ConsoleAction]
        public void UnsupportedMethod1(bool arg)
        {
            throw new AssertionException("Should not get here");
        }

        [ConsoleAction]
        public bool UnsupportedMethod2()
        {
            throw new AssertionException("Should not get here");
        }

        [ConsoleAction]
        public IEnumerator UnsupportedCoroutine()
        {
            throw new AssertionException("Should not get here");
        }

        public Delegate privateAction
        {
            get
            {
                Action action = PrivateAction;
                return action;
            }
        }

        public static Delegate privateStaticAction
        {
            get
            {
                Action action = PrivateStaticAction;
                return action;
            }
        }
    }
}