using System;
using System.IO;
using NUnit.Framework;

using Org.System.Xml.Sax;

using EXIDecoder = Nagasena.Proc.EXIDecoder;
using AlignmentType = Nagasena.Proc.Common.AlignmentType;
using EventDescription = Nagasena.Proc.Common.EventDescription;
using EventDescription_Fields = Nagasena.Proc.Common.EventDescription_Fields;
using EventType = Nagasena.Proc.Common.EventType;
using EventTypeList = Nagasena.Proc.Common.EventTypeList;
using GrammarOptions = Nagasena.Proc.Common.GrammarOptions;
using GrammarCache = Nagasena.Proc.Grammars.GrammarCache;
using Scanner = Nagasena.Proc.IO.Scanner;
using EXISchema = Nagasena.Schema.EXISchema;
using TestBase = Nagasena.Schema.TestBase;
using EXISchemaFactoryTestUtil = Nagasena.Scomp.EXISchemaFactoryTestUtil;
using System.Windows.Forms;

namespace Nagasena.Sax
{

    [TestFixture]
    public class ISO15118Test : TestBase
    {

        private static readonly AlignmentType[] Alignments = new AlignmentType[] {
      AlignmentType.bitPacked,
      AlignmentType.byteAligned,
      AlignmentType.preCompress,
      AlignmentType.compress
    };

        ///////////////////////////////////////////////////////////////////////////
        // Test cases
        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// EXI test cases of ISO15118.
        /// </summary>
        [Test]
        public virtual void testISO15118()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/iso15118/V2G_CI_MsgDef.gram", this);

            //GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.STRICT_OPTIONS);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.DEFAULT_OPTIONS);

            foreach (AlignmentType alignment in Alignments)
            {
                Transmogrifier encoder = new Transmogrifier();
                EXIDecoder decoder = new EXIDecoder();
                Scanner scanner;
                InputSource inputSource;

                encoder.AlignmentType = alignment;
                decoder.AlignmentType = alignment;

                encoder.GrammarCache = grammarCache;
                MemoryStream baos = new MemoryStream();
                encoder.OutputStream = baos;

                //Uri url = resolveSystemIdAsURL("/iso15118/SessionSetupRes_openEXI.xml");
                Uri url = resolveSystemIdAsURL("/iso15118/SessionSetupRes_openV2G_space.xml");
                FileStream inputStream = new FileStream(url.LocalPath, FileMode.Open);
                inputSource = new InputSource<Stream>(inputStream, url.ToString());

                byte[] bts;
                int n_events;

                encoder.encode(inputSource);
                inputStream.Close();

                bts = baos.ToArray();

                decoder.GrammarCache = grammarCache;
                decoder.InputStream = new MemoryStream(bts);
                scanner = decoder.processHeader();

                EventDescription exiEvent;
                n_events = 0;

                EventType eventType;
                EventTypeList eventTypeList;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
                eventType = exiEvent.getEventType();
                Assert.AreSame(exiEvent, eventType);
                Assert.AreEqual(0, eventType.Index);
                eventTypeList = eventType.EventTypeList;
                Assert.AreEqual(1, eventTypeList.Length);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
                Assert.AreEqual("V2G_Message", exiEvent.Name);
                Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
                Assert.AreEqual("Header", exiEvent.Name);
                Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
                Assert.AreEqual("SessionID", exiEvent.Name);
                Assert.AreEqual("urn:iso:15118:2:2013:MsgHeader", exiEvent.URI);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
                //Assert.AreEqual("3031323334353637", exiEvent.Characters.makeString());
                Assert.AreEqual("010203040506", exiEvent.Characters.makeString());
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
                Assert.AreEqual("Body", exiEvent.Name);
                Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
                Assert.AreEqual("SessionSetupRes", exiEvent.Name);
                Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
                Assert.AreEqual("ResponseCode", exiEvent.Name);
                Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
                Assert.AreEqual("OK", exiEvent.Characters.makeString());
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
                Assert.AreEqual("EVSEID", exiEvent.Name);
                Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
                //Assert.AreEqual("FRA23E45B78C", exiEvent.Characters.makeString());
                Assert.AreEqual("ABC01234", exiEvent.Characters.makeString());
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
                ++n_events;

                exiEvent = scanner.nextEvent();
                Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
                ++n_events;

                Assert.IsNull(scanner.nextEvent());

                Assert.AreEqual(19, n_events);
            }
        }


        public virtual void testAuthorizationReqMessage()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/iso15118/V2G_CI_MsgDef.gram", this);


            //GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.STRICT_OPTIONS);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.STRICT_OPTIONS);

            AlignmentType alignment = Alignments[0];            // bitPacked
            Transmogrifier encoder = new Transmogrifier();
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;
            InputSource inputSource;

            encoder.AlignmentType = alignment;
            decoder.AlignmentType = alignment;

            encoder.GrammarCache = grammarCache;
            MemoryStream baos = new MemoryStream();
            encoder.OutputStream = baos;

            //Uri url = resolveSystemIdAsURL("/iso15118/SessionSetupRes.xml");
            //FileStream inputStream = new FileStream(url.LocalPath, FileMode.Open);
            //inputSource = new InputSource<Stream>(inputStream, url.ToString());

            // PNC_Charging_OK.txt
            // SessionSetupRes
            byte[] bts = { 0x80, 0x98, 0x02, 0x03, 0x00, 0x00, 0x00, 0x2d, 0xab, 0x49, 0x56, 0x11, 0xe0, 0x20, 0x45, 0x2d, 0x48, 0xa9, 0x1d, 0x5d, 0x68, 0xa9, 0x14, 0xd0, 0xd5, 0x08, 0xa8, 0xdc, 0xe1, 0x0c, 0x16, 0xdb, 0x72, 0xd8, 0x20, 0xa0 };

            // openV2G sample : sessionSetupRes.xml.exi
            //byte[] bts = { 0x80, 0x98, 0x01, 0x80, 0x40, 0x80, 0xC1, 0x01, 0x41, 0x91, 0xE0, 0x00, 0x29, 0x05, 0x09, 0x0C, 0xC0, 0xC4, 0xC8, 0xCC, 0xD0, 0x12, 0xB3, 0x5D, 0xE7, 0x40 };
            //byte[] bts = { 0x80, 0x98, 0x0C, 0x02, 0x04, 0x06, 0x08, 0x0a, 0x0d, 0x3c, 0x00, 0xa4, 0x14, 0x24, 0x33, 0x03, 0x13, 0x23, 0x33, 0x41, 0x2b, 0x35, 0xde, 0x74 };
            //byte[] bts = { 0x80, 0x04, 0x01, 0x52, 0x51, 0x0C, 0x40, 0x82, 0x9B, 0x7B, 0x6B, 0x29, 0x02, 0x93, 0x0B, 0x73, 0x23, 0x7B, 0x69, 0x02, 0x23, 0x0B, 0xA3, 0x09, 0xE8 };
            int n_events;

            //encoder.encode(inputSource);
            //inputStream.Close();

            //bts = baos.ToArray();

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);
            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            eventType = exiEvent.getEventType();
            Assert.AreSame(exiEvent, eventType);
            Assert.AreEqual(0, eventType.Index);
            eventTypeList = eventType.EventTypeList;
            Assert.AreEqual(1, eventTypeList.Length);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("V2G_Message", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Header", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionID", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgHeader", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("010203040506", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Body", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionSetupRes", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ResponseCode", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("OK", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVSEID", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("ABC01234", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
            ++n_events;

            Assert.IsNull(scanner.nextEvent());

            Assert.AreEqual(19, n_events);
        }

        public virtual void supportedAppProtocolRes()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/din/V2G_CI_MsgDef.exig", this);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.DEFAULT_OPTIONS);

            AlignmentType alignment = Alignments[0];            // bitPacked
            Transmogrifier encoder = new Transmogrifier();
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;
            InputSource inputSource;

            encoder.AlignmentType = alignment;
            decoder.AlignmentType = alignment;

            encoder.GrammarCache = grammarCache;
            MemoryStream baos = new MemoryStream();
            encoder.OutputStream = baos;

            Uri url = resolveSystemIdAsURL("/din/supportedAppProtocolRes.xml");
            FileStream inputStream = new FileStream(url.LocalPath, FileMode.Open);
            inputSource = new InputSource<Stream>(inputStream, url.ToString());

            byte[] bts;
            int n_events;

            encoder.encode(inputSource);
            inputStream.Close();

            bts = baos.ToArray();

            foreach(var item in bts)
            {
                Console.Write("{0:X2} ", item);
            }

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);
            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            eventType = exiEvent.getEventType();
            Assert.AreSame(exiEvent, eventType);
            Assert.AreEqual(0, eventType.Index);
            eventTypeList = eventType.EventTypeList;
            Assert.AreEqual(1, eventTypeList.Length);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("supportedAppProtocolRes", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2010:AppProtocol", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ResponseCode", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("OK_SuccessfulNegotiationWithMinorDeviation", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SchemaID", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("1", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
            ++n_events;

            Assert.IsNull(scanner.nextEvent());
        }

        public virtual void supportedAppProtocolReq()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/din/V2G_CI_AppProtocol.exig", this);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.DEFAULT_OPTIONS);

            AlignmentType alignment = Alignments[0];            // bitPacked
            Transmogrifier encoder = new Transmogrifier();
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;
            InputSource inputSource;

            encoder.AlignmentType = alignment;
            decoder.AlignmentType = alignment;

            encoder.GrammarCache = grammarCache;
            MemoryStream baos = new MemoryStream();
            encoder.OutputStream = baos;

            Uri url = resolveSystemIdAsURL("/din/supportedAppProtocolReq.xml");
            FileStream inputStream = new FileStream(url.LocalPath, FileMode.Open);
            inputSource = new InputSource<Stream>(inputStream, url.ToString());

            byte[] bts;
            int n_events;

            encoder.encode(inputSource);
            inputStream.Close();

            bts = baos.ToArray();

            foreach (var item in bts)
            {
                Console.Write("{0:X2} ", item);
            }

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);
            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            eventType = exiEvent.getEventType();
            Assert.AreSame(exiEvent, eventType);
            Assert.AreEqual(0, eventType.Index);
            eventTypeList = eventType.EventTypeList;
            Assert.AreEqual(1, eventTypeList.Length);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("supportedAppProtocolReq", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2010:AppProtocol", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("AppProtocol", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ProtocolNamespace", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("VersionNumberMajor", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("2", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("VersionNumberMinor", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("0", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SchemaID", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("10", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Priority", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("1", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("AppProtocol", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ProtocolNamespace", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("urn:iso:15118:2:2010:MsgDef", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("VersionNumberMajor", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("1", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("VersionNumberMinor", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("0", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SchemaID", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("20", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Priority", exiEvent.Name);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("5", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
            ++n_events;

            Assert.IsNull(scanner.nextEvent());
        }

        public virtual void SessionReq()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/din/V2G_CI_MsgBody.exig", this);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.DEFAULT_OPTIONS);

            AlignmentType alignment = AlignmentType.bitPacked;         // bitPacked
            Transmogrifier encoder = new Transmogrifier();
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;
            InputSource inputSource;

            encoder.AlignmentType = alignment;
            encoder.ValuePartitionCapacity = 0;
            encoder.Fragment = false;
            encoder.BlockSize = 1000000;

            decoder.AlignmentType = alignment;
            decoder.ValuePartitionCapacity = 0;

            encoder.GrammarCache = grammarCache;
            MemoryStream baos = new MemoryStream();
            encoder.OutputStream = baos;

            Uri url = resolveSystemIdAsURL("/din/SessionSetupRes.xml");
            FileStream inputStream = new FileStream(url.LocalPath, FileMode.Open);
            inputSource = new InputSource<Stream>(inputStream, url.ToString());

            byte[] bts;
            int n_events;

            encoder.encode(inputSource);
            inputStream.Close();

            bts = baos.ToArray();

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);

            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            eventType = exiEvent.getEventType();
            Assert.AreSame(exiEvent, eventType);
            Assert.AreEqual(0, eventType.Index);
            eventTypeList = eventType.EventTypeList;
            Assert.AreEqual(1, eventTypeList.Length);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("V2G_Message", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Header", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionID", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgHeader", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("010203040506", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Body", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionSetupRes", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ResponseCode", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("OK", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVSEID", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("ABC01234", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVSETimeStamp", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("123456789", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
            ++n_events;

            Assert.IsNull(scanner.nextEvent());

            Assert.AreEqual(22, n_events);

            MessageBox.Show("Complete");
        }

        public virtual void BMWi3_CableCheckReq()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/din/V2G_CI_MsgDef.exig", this);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.DEFAULT_OPTIONS);

            AlignmentType alignment = Alignments[0];            // bitPacked
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;

            //Transmogrifier encoder = new Transmogrifier();
            //encoder.AlignmentType = alignment;
            //encoder.Fragment = false;
            //encoder.ValuePartitionCapacity = 0;
            //encoder.BlockSize = 1000000;
            //encoder.GrammarCache = grammarCache;
            //encoder.OutputStream = baos;

            decoder.AlignmentType = alignment;
            decoder.Fragment = false;
            decoder.ValuePartitionCapacity = 0;
            decoder.BlockSize = 1000000;

            MemoryStream baos = new MemoryStream();

            byte[] bts =
            {
                0x80, 0x9a, 0x02, 0x02, 0x00, 0x00, 0x00, 0x1a, 0x07, 0x40, 0x00, 0x10, 0x11, 0x08, 0x40, 0x0a, 0x00
            };

            int n_events;

            //bts = baos.ToArray();

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);

            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            eventType = exiEvent.getEventType();
            Assert.AreSame(exiEvent, eventType);
            Assert.AreEqual(0, eventType.Index);
            eventTypeList = eventType.EventTypeList;
            Assert.AreEqual(1, eventTypeList.Length);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("V2G_Message", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Header", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionID", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgHeader", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("08000000681D0000", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Body", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("CableCheckReq", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("DC_EVStatus", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVReady", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("true", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVCabinConditioning", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("true", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVRESSConditioning", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("true", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVErrorCode", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("NO_ERROR", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVRESSSOC", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("80", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
            ++n_events;

            MessageBox.Show("Complete");
        }

        public virtual void BMWi3_ServiceDiscoveryReq()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/din/V2G_CI_MsgDef.exig", this);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.DEFAULT_OPTIONS);

            AlignmentType alignment = Alignments[0];            // bitPacked
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;

            decoder.AlignmentType = alignment;
            decoder.Fragment = false;
            decoder.ValuePartitionCapacity = 0;
            decoder.BlockSize = 1000000;

            MemoryStream baos = new MemoryStream();

            byte[] bts =
            {
                0x80, 0x9a, 0x02, 0x02, 0x00, 0x00, 0x00, 0x1a, 0x07, 0x40, 0x00,
                0x11, 0x90, 0x04, 0x00, 0x00
            };

            int n_events;

            //bts = baos.ToArray();

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);

            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            eventType = exiEvent.getEventType();
            Assert.AreSame(exiEvent, eventType);
            Assert.AreEqual(0, eventType.Index);
            eventTypeList = eventType.EventTypeList;
            Assert.AreEqual(1, eventTypeList.Length);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("V2G_Message", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Header", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionID", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgHeader", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("08000000681D0000", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Body", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ServiceDiscoveryReq", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ServiceScope", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ServiceCategory", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("EVCharging", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
            ++n_events;

            MessageBox.Show("Complete");
        }

        public virtual void BMWi3_CurrentDemandReq()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/din/V2G_CI_MsgDef.exig", this);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.DEFAULT_OPTIONS);

            AlignmentType alignment = Alignments[0];            // bitPacked
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;

            decoder.AlignmentType = alignment;
            decoder.Fragment = false;
            decoder.ValuePartitionCapacity = 0;
            decoder.BlockSize = 1000000;

            MemoryStream baos = new MemoryStream();

            byte[] bts =
            {
                0x80, 0x9a, 0x02, 0x02, 0x00, 0x00, 0x00, 0x1a, 0x07, 0x40, 0x00,
                0x10, 0xd1, 0x08,
                0x40, 0x0a, 0x00, 0x60, 0x60, 0x0a, 0x01, 0x02, 0x85, 0x10, 0xf8, 0x06, 0x06, 0x0f, 0xa2, 0x80,
                0x20, 0x10, 0x55, 0x03, 0x01, 0x00, 0x80, 0x00, 0x08, 0x14, 0x28, 0x87, 0xc0
            };

            int n_events;

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);

            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            eventType = exiEvent.getEventType();
            Assert.AreSame(exiEvent, eventType);
            Assert.AreEqual(0, eventType.Index);
            eventTypeList = eventType.EventTypeList;
            Assert.AreEqual(1, eventTypeList.Length);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("V2G_Message", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Header", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionID", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgHeader", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("08000000681D0000", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Body", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("CurrentDemandReq", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("DC_EVStatus", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVReady", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("true", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVCabinConditioning", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("true", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVRESSConditioning", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("true", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVErrorCode", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("NO_ERROR", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVRESSSOC", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("80", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVTargetCurrent", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Multiplier", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("0", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Unit", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("A", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Value", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("5", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVMaximumVoltageLimit", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Multiplier", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("-1", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Unit", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("V", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Value", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("4002", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVMaximumCurrentLimit", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Multiplier", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("0", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Unit", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("A", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Value", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("125", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("BulkChargingComplete", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("true", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ChargingComplete", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("false", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("RemainingTimeToFullSoC", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Multiplier", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("1", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Unit", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("s", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Value", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("810", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("RemainingTimeToBulkSoC", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Multiplier", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("1", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Unit", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("s", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Value", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("0", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVTargetVoltage", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Multiplier", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("-1", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Unit", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("V", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Value", exiEvent.Name);
            Assert.AreEqual("urn:din:70121:2012:MsgDataTypes", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("4002", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
            ++n_events;

            MessageBox.Show("Complete");
        }

        public void EXISchemaTest()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/iso15118/V2G_CI_MsgDef.gram", this);

            //GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.STRICT_OPTIONS);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.STRICT_OPTIONS);

            AlignmentType alignment = Alignments[0];            // bitPacked
            Transmogrifier encoder = new Transmogrifier();
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;
            InputSource inputSource;

            encoder.AlignmentType = alignment;
            decoder.AlignmentType = alignment;

            encoder.GrammarCache = grammarCache;
            MemoryStream baos = new MemoryStream();
            encoder.OutputStream = baos;

            //Uri url = resolveSystemIdAsURL("/iso15118/SessionSetupRes.xml");
            //FileStream inputStream = new FileStream(url.LocalPath, FileMode.Open);
            //inputSource = new InputSource<Stream>(inputStream, url.ToString());

            // PNC_Charging_OK.txt
            // SessionSetupRes
            byte[] bts = { 0x80, 0x98, 0x02, 0x03, 0x00, 0x00, 0x00, 0x2d, 0xab, 0x49, 0x56, 0x11, 0xe0, 0x20, 0x45, 0x2d, 0x48, 0xa9, 0x1d, 0x5d, 0x68, 0xa9, 0x14, 0xd0, 0xd5, 0x08, 0xa8, 0xdc, 0xe1, 0x0c, 0x16, 0xdb, 0x72, 0xd8, 0x20, 0xa0 };

            // openV2G sample : sessionSetupRes.xml.exi
            //byte[] bts = { 0x80, 0x98, 0x01, 0x80, 0x40, 0x80, 0xC1, 0x01, 0x41, 0x91, 0xE0, 0x00, 0x29, 0x05, 0x09, 0x0C, 0xC0, 0xC4, 0xC8, 0xCC, 0xD0, 0x12, 0xB3, 0x5D, 0xE7, 0x40 };
            //byte[] bts = { 0x80, 0x98, 0x0C, 0x02, 0x04, 0x06, 0x08, 0x0a, 0x0d, 0x3c, 0x00, 0xa4, 0x14, 0x24, 0x33, 0x03, 0x13, 0x23, 0x33, 0x41, 0x2b, 0x35, 0xde, 0x74 };
            //byte[] bts = { 0x80, 0x04, 0x01, 0x52, 0x51, 0x0C, 0x40, 0x82, 0x9B, 0x7B, 0x6B, 0x29, 0x02, 0x93, 0x0B, 0x73, 0x23, 0x7B, 0x69, 0x02, 0x23, 0x0B, 0xA3, 0x09, 0xE8 };
            int n_events;

            //encoder.encode(inputSource);
            //inputStream.Close();

            //bts = baos.ToArray();

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);
            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            //while ( true )
            //{
            //    exiEvent = scanner.nextEvent();
            //    Console.WriteLine(exiEvent.EventKind);

            //    if (exiEvent.EventKind == EventDescription_Fields.EVENT_ED)
            //    {
            //        break;
            //    }

            //    ++n_events;

            //    // 0 2 2 2 6 7 7 2 2 
            //}

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            eventType = exiEvent.getEventType();
            Assert.AreSame(exiEvent, eventType);
            Assert.AreEqual(0, eventType.Index);
            eventTypeList = eventType.EventTypeList;
            Assert.AreEqual(1, eventTypeList.Length);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("V2G_Message", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Header", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionID", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgHeader", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("010203040506", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("Body", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgDef", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("SessionSetupRes", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("ResponseCode", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("OK", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_SE, exiEvent.EventKind);
            Assert.AreEqual("EVSEID", exiEvent.Name);
            Assert.AreEqual("urn:iso:15118:2:2013:MsgBody", exiEvent.URI);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_CH, exiEvent.EventKind);
            Assert.AreEqual("ABC01234", exiEvent.Characters.makeString());
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_EE, exiEvent.EventKind);
            ++n_events;

            exiEvent = scanner.nextEvent();
            Assert.AreEqual(EventDescription_Fields.EVENT_ED, exiEvent.EventKind);
            ++n_events;

            Assert.IsNull(scanner.nextEvent());

            Assert.AreEqual(19, n_events);
        }

        public void test15118Signature()
        {
            EXISchema corpus = EXISchemaFactoryTestUtil.getEXISchema("/iso15118/V2G_CI_MsgDef.gram", this);
            GrammarCache grammarCache = new GrammarCache(corpus, GrammarOptions.DEFAULT_OPTIONS);

            AlignmentType alignment = AlignmentType.bitPacked;        // bitPacked
            Transmogrifier encoder = new Transmogrifier();
            EXIDecoder decoder = new EXIDecoder();
            Scanner scanner;
            InputSource inputSource;

            encoder.Fragment = true;
            encoder.AlignmentType = alignment;
            decoder.AlignmentType = alignment;

            encoder.GrammarCache = grammarCache;
            MemoryStream baos = new MemoryStream();
            encoder.OutputStream = baos;

            Uri url = resolveSystemIdAsURL("/iso15118/SalesTariff.xml");
            FileStream inputStream = new FileStream(url.LocalPath, FileMode.Open);
            inputSource = new InputSource<Stream>(inputStream, url.ToString());

            byte[] bts;
            int n_events;

            encoder.encode(inputSource);
            inputStream.Close();

            bts = baos.ToArray();

            foreach(var item in bts)
            {
                Console.Write("{0:X2} ", item);
            }

            Console.WriteLine("EXI Length " + bts.Length.ToString());

            decoder.GrammarCache = grammarCache;
            decoder.InputStream = new MemoryStream(bts);
            scanner = decoder.processHeader();

            EventDescription exiEvent;
            n_events = 0;

            EventType eventType;
            EventTypeList eventTypeList;

            //exiEvent = scanner.nextEvent();
            //Assert.AreEqual(EventDescription_Fields.EVENT_SD, exiEvent.EventKind);
            //eventType = exiEvent.getEventType();
            //Assert.AreSame(exiEvent, eventType);
            //Assert.AreEqual(0, eventType.Index);
            //eventTypeList = eventType.EventTypeList;
            //Assert.AreEqual(1, eventTypeList.Length);
            //++n_events;
        }
    }
}