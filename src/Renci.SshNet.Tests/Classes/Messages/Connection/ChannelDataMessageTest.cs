﻿using System.Linq;
using Renci.SshNet.Common;
using Renci.SshNet.Messages.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Renci.SshNet.Tests.Common;

namespace Renci.SshNet.Tests.Classes.Messages.Connection
{
    /// <summary>
    ///This is a test class for ChannelDataMessageTest and is intended
    ///to contain all ChannelDataMessageTest Unit Tests
    ///</summary>
    [TestClass]
    public class ChannelDataMessageTest : TestBase
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            var target = new ChannelDataMessage();

            Assert.IsNull(target.Data);
#if TUNING
            Assert.AreEqual(0, target.Offset);
            Assert.AreEqual(0, target.Size);
#endif
        }

        [TestMethod]
        public void Constructor_LocalChannelNumberAndData()
        {
            var random = new Random();

            var localChannelNumber = (uint)random.Next(0, int.MaxValue);
            var data = new byte[3];

            var target = new ChannelDataMessage(localChannelNumber, data);

            Assert.AreSame(data, target.Data);
#if TUNING
            Assert.AreEqual(0, target.Offset);
            Assert.AreEqual(data.Length, target.Size);
#endif
        }

        [TestMethod]
        public void Constructor_LocalChannelNumberAndData_ShouldThrowArgumentNullExceptionWhenDataIsNull()
        {
            var localChannelNumber = (uint) new Random().Next(0, int.MaxValue);
            byte[] data = null;

            try
            {
                new ChannelDataMessage(localChannelNumber, data);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.InnerException);
                Assert.AreEqual("data", ex.ParamName);
            }
        }

#if TUNING
        [TestMethod]
        public void Constructor_LocalChannelNumberAndDataAndOffsetAndSize()
        {
            var localChannelNumber = (uint) new Random().Next(0, int.MaxValue);
            var data = new byte[4];
            const int offset = 2;
            const int size = 1;

            var target = new ChannelDataMessage(localChannelNumber, data, offset, size);

            Assert.AreSame(data, target.Data);
            Assert.AreEqual(offset, target.Offset);
            Assert.AreEqual(size, target.Size);
        }

        [TestMethod]
        public void Constructor_LocalChannelNumberAndDataAndOffsetAndSize_ShouldThrowArgumentNullExceptionWhenDataIsNull()
        {
            var localChannelNumber = (uint) new Random().Next(0, int.MaxValue);
            byte[] data = null;
            const int offset = 0;
            const int size = 0;

            try
            {
                new ChannelDataMessage(localChannelNumber, data, offset, size);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.InnerException);
                Assert.AreEqual("data", ex.ParamName);
            }
        }
#endif

        [TestMethod]
        public void GetBytes()
        {
            var random = new Random();

            var localChannelNumber = (uint) random.Next(0, int.MaxValue);
            var data = new byte[random.Next(10, 20)];
            random.NextBytes(data);
#if TUNING
            var offset = random.Next(2, 4);
            var size = random.Next(5, 9);

            var target = new ChannelDataMessage(localChannelNumber, data, offset, size);
#else
            var offset = 0;
            var size = data.Length;

            var target = new ChannelDataMessage(localChannelNumber, data);
#endif

            var bytes = target.GetBytes();

            var expectedBytesLength = 1; // Type
            expectedBytesLength += 4; // LocalChannelNumber
            expectedBytesLength += 4; // Data length
            expectedBytesLength += size; // Data

            Assert.AreEqual(expectedBytesLength, bytes.Length);

            var sshDataStream = new SshDataStream(bytes);

            Assert.AreEqual(ChannelDataMessage.MessageNumber, sshDataStream.ReadByte());
            Assert.AreEqual(localChannelNumber, sshDataStream.ReadUInt32());
            Assert.AreEqual((uint) size, sshDataStream.ReadUInt32());

            var actualData = new byte[size];
            sshDataStream.Read(actualData, 0, size);
            Assert.IsTrue(actualData.SequenceEqual(data.Take(offset, size)));

            Assert.IsTrue(sshDataStream.IsEndOfData);
        }

        [TestMethod]
        public void Load()
        {
            var random = new Random();

            var localChannelNumber = (uint)random.Next(0, int.MaxValue);
            var data = new byte[random.Next(10, 20)];
            random.NextBytes(data);
#if TUNING
            var offset = random.Next(2, 4);
            var size = random.Next(5, 9);
            var channelDataMessage = new ChannelDataMessage(localChannelNumber, data, offset, size);
#else
            var offset = 0;
            var size = data.Length;
            var channelDataMessage = new ChannelDataMessage(localChannelNumber, data);
#endif
            var bytes = channelDataMessage.GetBytes();
            var target = new ChannelDataMessage();

            target.Load(bytes);

            Assert.IsTrue(target.Data.SequenceEqual(data.Take(offset, size)));
#if TUNING
            Assert.AreEqual(0, target.Offset);
            Assert.AreEqual(size, target.Size);
#endif
        }
    }
}