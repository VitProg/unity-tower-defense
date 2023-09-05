using UnityEngine.TestTools;
using NUnit.Framework;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using UnityEngine;

#region components
struct CC01 {}
struct CC02 {}
struct CC03 {}
struct CC04 {}
struct CC05 {}
struct CC06 {}
struct CC07 {}
struct CC08 {}
struct CC09 {}
struct CC10 {}
struct CC11 {}
struct CC12 {}
struct CC13 {}
struct CC14 {}
struct CC15 {}
struct CC16 {}
struct CC17 {}
struct CC18 {}
struct CC19 {}
struct CC20 {}
struct CC21 {}
struct CC22 {}
struct CC23 {}
struct CC24 {}
struct CC25 {}
struct CC26 {}
struct CC27 {}
struct CC28 {}
struct CC29 {}
struct CC30 {}
struct CC31 {}
struct CC32 {}
struct CC33 {}
struct CC34 {}
struct CC35 {}
struct CC36 {}
struct CC37 {}
struct CC38 {}
struct CC39 {}
struct CC40 {}
struct CC41 {}
struct CC42 {}
struct CC43 {}
struct CC44 {}
struct CC45 {}
struct CC46 {}
struct CC47 {}
struct CC48 {}
struct CC49 {}
struct CC50 {}
struct CC51 {}
struct CC52 {}
struct CC53 {}
struct CC54 {}
struct CC55 {}
struct CC56 {}
struct CC57 {}
struct CC58 {}
struct CC59 {}
struct CC60 {}
struct CC61 {}
struct CC62 {}
struct CC63 {}
struct CC64 {}
struct CC65 {}
struct CC66 {}
struct CC67 {}
struct CC68 {}
struct CC69 {}
struct CC70 {}
struct CC71 {}
struct CC72 {}
struct CC73 {}
struct CC74 {}
struct CC75 {}
struct CC76 {}
struct CC77 {}
struct CC78 {}
struct CC79 {}
struct CC80 {}
struct CC81 {}
struct CC82 {}
struct CC83 {}
struct CC84 {}
struct CC85 {}
struct CC86 {}
struct CC87 {}
struct CC88 {}
struct CC89 {}
struct CC90 {}
struct CC91 {}
struct CC92 {}
struct CC93 {}
struct CC94 {}
struct CC95 {}
struct CC96 {}
struct CC97 {}
struct CC98 {}
struct CC99 {}
struct CC100 {}
struct CC101 {}
struct CC102 {}
struct CC103 {}
struct CC104 {}
struct CC105 {}
struct CC106 {}
struct CC107 {}
struct CC108 {}
struct CC109 {}
struct CC110 {}
struct CC111 {}
struct CC112 {}
struct CC113 {}
struct CC114 {}
struct CC115 {}
struct CC116 {}
struct CC117 {}
struct CC118 {}
struct CC119 {}
struct CC120 {}
struct CC121 {}
struct CC122 {}
struct CC123 {}
struct CC124 {}
struct CC125 {}
struct CC126 {}
struct CC127 {}
struct CC128 {}
struct CC129 {}
struct CC130 {}
#endregion

class TestSuite {
    class AspectWith130Components : ProtoAspectInject {
        public ProtoPool<CC01> CC01;
        public ProtoPool<CC02> CC02;
        public ProtoPool<CC03> CC03;
        public ProtoPool<CC04> CC04;
        public ProtoPool<CC05> CC05;
        public ProtoPool<CC06> CC06;
        public ProtoPool<CC07> CC07;
        public ProtoPool<CC08> CC08;
        public ProtoPool<CC09> CC09;
        public ProtoPool<CC10> CC10;
        public ProtoPool<CC11> CC11;
        public ProtoPool<CC12> CC12;
        public ProtoPool<CC13> CC13;
        public ProtoPool<CC14> CC14;
        public ProtoPool<CC15> CC15;
        public ProtoPool<CC16> CC16;
        public ProtoPool<CC17> CC17;
        public ProtoPool<CC18> CC18;
        public ProtoPool<CC19> CC19;
        public ProtoPool<CC20> CC20;
        public ProtoPool<CC21> CC21;
        public ProtoPool<CC22> CC22;
        public ProtoPool<CC23> CC23;
        public ProtoPool<CC24> CC24;
        public ProtoPool<CC25> CC25;
        public ProtoPool<CC26> CC26;
        public ProtoPool<CC27> CC27;
        public ProtoPool<CC28> CC28;
        public ProtoPool<CC29> CC29;
        public ProtoPool<CC30> CC30;
        public ProtoPool<CC31> CC31;
        public ProtoPool<CC32> CC32;
        public ProtoPool<CC33> CC33;
        public ProtoPool<CC34> CC34;
        public ProtoPool<CC35> CC35;
        public ProtoPool<CC36> CC36;
        public ProtoPool<CC37> CC37;
        public ProtoPool<CC38> CC38;
        public ProtoPool<CC39> CC39;
        public ProtoPool<CC40> CC40;
        public ProtoPool<CC41> CC41;
        public ProtoPool<CC42> CC42;
        public ProtoPool<CC43> CC43;
        public ProtoPool<CC44> CC44;
        public ProtoPool<CC45> CC45;
        public ProtoPool<CC46> CC46;
        public ProtoPool<CC47> CC47;
        public ProtoPool<CC48> CC48;
        public ProtoPool<CC49> CC49;
        public ProtoPool<CC50> CC50;
        public ProtoPool<CC51> CC51;
        public ProtoPool<CC52> CC52;
        public ProtoPool<CC53> CC53;
        public ProtoPool<CC54> CC54;
        public ProtoPool<CC55> CC55;
        public ProtoPool<CC56> CC56;
        public ProtoPool<CC57> CC57;
        public ProtoPool<CC58> CC58;
        public ProtoPool<CC59> CC59;
        public ProtoPool<CC60> CC60;
        public ProtoPool<CC61> CC61;
        public ProtoPool<CC62> CC62;
        public ProtoPool<CC63> CC63;
        public ProtoPool<CC64> CC64;
        public ProtoPool<CC65> CC65;
        public ProtoPool<CC66> CC66;
        public ProtoPool<CC67> CC67;
        public ProtoPool<CC68> CC68;
        public ProtoPool<CC69> CC69;
        public ProtoPool<CC70> CC70;
        public ProtoPool<CC71> CC71;
        public ProtoPool<CC72> CC72;
        public ProtoPool<CC73> CC73;
        public ProtoPool<CC74> CC74;
        public ProtoPool<CC75> CC75;
        public ProtoPool<CC76> CC76;
        public ProtoPool<CC77> CC77;
        public ProtoPool<CC78> CC78;
        public ProtoPool<CC79> CC79;
        public ProtoPool<CC80> CC80;
        public ProtoPool<CC81> CC81;
        public ProtoPool<CC82> CC82;
        public ProtoPool<CC83> CC83;
        public ProtoPool<CC84> CC84;
        public ProtoPool<CC85> CC85;
        public ProtoPool<CC86> CC86;
        public ProtoPool<CC87> CC87;
        public ProtoPool<CC88> CC88;
        public ProtoPool<CC89> CC89;
        public ProtoPool<CC90> CC90;
        public ProtoPool<CC91> CC91;
        public ProtoPool<CC92> CC92;
        public ProtoPool<CC93> CC93;
        public ProtoPool<CC94> CC94;
        public ProtoPool<CC95> CC95;
        public ProtoPool<CC96> CC96;
        public ProtoPool<CC97> CC97;
        public ProtoPool<CC98> CC98;
        public ProtoPool<CC99> CC99;
        public ProtoPool<CC100> CC100;
        public ProtoPool<CC101> CC101;
        public ProtoPool<CC102> CC102;
        public ProtoPool<CC103> CC103;
        public ProtoPool<CC104> CC104;
        public ProtoPool<CC105> CC105;
        public ProtoPool<CC106> CC106;
        public ProtoPool<CC107> CC107;
        public ProtoPool<CC108> CC108;
        public ProtoPool<CC109> CC109;
        public ProtoPool<CC110> CC110;
        public ProtoPool<CC111> CC111;
        public ProtoPool<CC112> CC112;
        public ProtoPool<CC113> CC113;
        public ProtoPool<CC114> CC114;
        public ProtoPool<CC115> CC115;
        public ProtoPool<CC116> CC116;
        public ProtoPool<CC117> CC117;
        public ProtoPool<CC118> CC118;
        public ProtoPool<CC119> CC119;
        public ProtoPool<CC120> CC120;
        public ProtoPool<CC121> CC121;
        public ProtoPool<CC122> CC122;
        public ProtoPool<CC123> CC123;
        public ProtoPool<CC124> CC124;
        public ProtoPool<CC125> CC125;
        public ProtoPool<CC126> CC126;
        public ProtoPool<CC127> CC127;
        public ProtoPool<CC128> CC128;
        public ProtoPool<CC129> CC129;
        public ProtoPool<CC130> CC130;

        public ProtoIt it = new ProtoIt(It.Inc<CC100>());
        public ProtoIt it2 = new ProtoIt(It.Inc<CC11, CC100>());
    }

    [Test]
    public void TestCase1() {
        var aspect = new AspectWith130Components();
        var world = new ProtoWorld(aspect);
        Assert.AreEqual(130, world.Pools().Len());
        
        var e1 = aspect.World().NewEntity();
        aspect.CC10.Add(e1);
        aspect.CC100.Add(e1);
        aspect.CC130.Add(e1);
        
        var e2 = aspect.World().NewEntity();
        aspect.CC11.Add(e2);
        aspect.CC100.Add(e2);
        for (var i = 0; i < 999; i++) {
            var e = aspect.World().NewEntity();
            aspect.CC11.Add(e);
            aspect.CC100.Add(e);
        }
        
        var data1 = world.Entities().Get(e1);
        Assert.AreEqual(data1.Mask.Len(), data1.Mask.Data().Length);
        Assert.AreEqual(3, data1.Mask.Data().Length);
        Assert.AreEqual(3, data1.Mask.Len());
        
        var data2 = world.Entities().Get(e2);
        Debug.Log("data2.Mask.Len() = " + data2.Mask.Len());
        Debug.Log("data2.Mask.Data().Length = " + data2.Mask.Data().Length);
        // Assert.AreEqual(data2.Mask.Len(), data2.Mask.Data().Length);
        // Assert.AreEqual(2, data2.Mask.Data().Length);
        // Assert.AreEqual(2, data2.Mask.Len());
        
        var count = 0;
        foreach (var i in aspect.Iter()) {
            count++;
        }
        Assert.AreEqual(0, count);

        count = 0;
        foreach (var i in aspect.it) {
            count++;
        }
        Assert.AreEqual(1001, count);
        
        count = 0;
        foreach (var i in aspect.it2) {
            count++;
        }
        Assert.AreEqual(1000, count);
        
        world.DelEntity(e1);
        world.DelEntity(e2);
        world.Destroy();
    }
}