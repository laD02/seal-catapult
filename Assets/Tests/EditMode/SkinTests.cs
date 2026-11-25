using System;
using System.Linq;
using NUnit.Framework;
using SealCatapult.Core;
using SealCatapult.Progress;

namespace SealCatapult.Tests
{
    public class SkinTests
    {
        [Test]
        public void EveryKindFitsOneShopPageAndHasOneClassic()
        {
            foreach (SkinKind kind in Enum.GetValues(typeof(SkinKind)))
            {
                var skins = Skins.Of(kind).ToList();
                Assert.That(skins.Count, Is.InRange(2, 9), $"{kind}: the shop grid is 3×3, no scrolling");
                Assert.AreEqual(1, skins.Count(s => s.IsClassic), $"{kind}: one free classic");
                Assert.AreSame(skins[0], Skins.Classic(kind), $"{kind}: the classic is the first card");
            }
            Assert.AreEqual(Skins.KindCount, Enum.GetValues(typeof(SkinKind)).Length);
        }

        [Test]
        public void EverySkinHasANameAndItsPicture()
        {
            foreach (var def in Skins.All)
            {
                Assert.IsTrue(Loc.Has(def.NameKey), $"name of {def.id}");
                if (def.kind == SkinKind.Launcher)
                {
                    // a drawn launcher names a set (base + arm); a repaint uses the classic pictures
                    Assert.IsTrue(Art.Has(Skins.CartBase(def)), $"cart of {def.id}");
                    Assert.IsTrue(Art.Has(Skins.CartArm(def)), $"arm of {def.id}");
                }
                else if (def.kind == SkinKind.Rope)
                    Assert.IsTrue(Art.Has(Skins.RopeSprite(def)), $"rope of {def.id}");
                else if (def.kind == SkinKind.Outfit)
                {
                    // a drawn outfit names its frame set: the frames the shop shows must be there
                    if (def.sprite != null) Assert.IsNotNull(Art.Plain(def.sprite + "_pup_idle_0"), $"standing frame of {def.id}");
                }
                else if (def.sprite != null) Assert.IsTrue(Art.Has(def.sprite), $"sprite {def.sprite} of {def.id}");
            }
            foreach (SkinKind kind in Enum.GetValues(typeof(SkinKind)))
                Assert.IsTrue(Loc.Has("hint_" + kind), $"hint of {kind}");
        }

        [Test]
        public void BuyingSpendsStarsAndPutsTheSkinOn()
        {
            var data = new SaveData { stars = new[] { 3, 3 } };
            var pink = Skins.Find("fur_pink");

            Assert.IsTrue(Skins.TryBuy(data, pink));

            Assert.AreEqual(6 - pink.price, Shop.AvailableStars(data));
            Assert.IsTrue(Skins.Owns(data, pink));
            Assert.AreSame(pink, Skins.Worn(data, SkinKind.Fur));
            Assert.IsFalse(Skins.TryBuy(data, pink), "not sold twice");
        }

        [Test]
        public void TooDearIsRefusedAndNothingChanges()
        {
            var data = new SaveData { stars = new[] { 1 } };
            var coal = Skins.Find("fur_coal");

            Assert.IsFalse(Skins.TryBuy(data, coal));

            Assert.AreEqual(1, Shop.AvailableStars(data));
            Assert.IsFalse(Skins.Owns(data, coal));
            Assert.IsFalse(Skins.Wear(data, coal), "an unowned skin cannot be worn");
            Assert.AreSame(Skins.Classic(SkinKind.Fur), Skins.Worn(data, SkinKind.Fur));
        }

        [Test]
        public void SkinsAndUpgradesShareOneWallet()
        {
            var data = new SaveData { stars = new[] { 3 }, spentStars = 2 };

            Assert.IsFalse(Skins.CanBuy(data, Skins.Find("fur_pink")), "2 stars: only 1 left after the upgrades");
            Assert.IsTrue(Skins.CanBuy(data, Skins.Find("fur_silver")));
        }

        [Test]
        public void TheClassicCanBePutBackOn()
        {
            var data = new SaveData { stars = new[] { 3, 3 } };
            Skins.TryBuy(data, Skins.Find("trail_stars"));

            Assert.IsTrue(Skins.Wear(data, Skins.Classic(SkinKind.Trail)));

            Assert.AreSame(Skins.Classic(SkinKind.Trail), Skins.Worn(data, SkinKind.Trail));
            Assert.IsTrue(Skins.Owns(data, Skins.Find("trail_stars")), "still owned");
        }

        [Test]
        public void ABadSaveNeverWearsWhatItDoesNotOwn()
        {
            var data = new SaveData
            {
                skins = new[] { "fur_pink", "fur_pink", "no_such_skin", null, "fur_classic" },
                worn = new[] { "fur_coal", "fur_pink", "garbage" },   // not owned, wrong kind, unknown
            };

            data.Normalize();

            CollectionAssert.AreEqual(new[] { "fur_pink" }, data.skins);
            Assert.AreEqual(Skins.KindCount, data.worn.Length);
            foreach (SkinKind kind in Enum.GetValues(typeof(SkinKind)))
                Assert.AreSame(Skins.Classic(kind), Skins.Worn(data, kind), kind.ToString());
        }

        [Test]
        public void AnOldSaveWithoutSkinFieldsLoadsAsClassic()
        {
            var data = UnityEngine.JsonUtility.FromJson<SaveData>("{\"version\":1,\"stars\":[3,2],\"spentStars\":1}");

            data.Normalize();

            Assert.IsEmpty(data.skins);
            Assert.AreSame(Skins.Classic(SkinKind.Fur), Skins.Worn(data, SkinKind.Fur));
            Assert.AreEqual(3, data.stars[0], "progress kept");
        }
    }
}
