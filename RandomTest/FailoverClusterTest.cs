﻿
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using NUnit.Framework;
using Easy.Rpc.LoadBalance;
using Easy.Rpc.Cluster;
using Easy.Rpc;
namespace RandomTest
{
	[TestFixture]
	public class FailoverClusterTest
	{
		[Test]
		public void FailoverClusterRetryTest()
		{			
			for (int i = 0; i < 100; i++) {
				Task.Factory.StartNew(() => { 
					String result = ClientFactory.Invoke(new TestInvoker(null, null));
					System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
					Assert.AreEqual("ok", result);
				                      
				});
			}
			
			Thread.Sleep(10000);
		}
		[Path("StaticDirectory", "order", "/oder/create")]
		[Cluster(FailoverCluster.NAME)]
		[LoadBalance(RandomBalance.NAME)]
		class TestInvoker:Invoker<String>
		{
			
			private int i = 0;
			public TestInvoker(object model, IDictionary<string, object> attachment)
				: base(model, attachment)
			{
				
			}

			#region implemented abstract members of Invoker
			public override string JoinURL(Node node, string path)
			{
				return node.Url + path;
			}
			#endregion
			public override String DoInvoke(Node node, string path)
			{
				System.IO.File.AppendAllText(@"F:\a.txt",node.Url+"\r\n");
				
				System.Diagnostics.Debug.WriteLine("调用次数：" + i);
				System.Diagnostics.Debug.WriteLine("调用的URL：" + node.Url);
				
				if (i == 0 || i == 1) {
					i++;
					throw new Exception();
				}
				
				if (i == 2) {
					return "ok";
				}
				
				
				return string.Empty;
			}
				
		}
	}
}
