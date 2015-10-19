﻿using System;
using System.Collections.Generic;
using System.Linq;
using Easy.Rpc.Cluster;
using Easy.Rpc.LoadBalance;
using Easy.Rpc.Exception;
using Easy.Rpc.directory;
namespace Easy.Rpc
{
	public class ClientInvoker
	{
		ClientInvoker()
		{
		}
		public static T Invoke<T>(IInvoker<T> invoker)
		{
			Object[] attributes = invoker.GetType().GetCustomAttributes(true);
			
			var directoryAttri = attributes.SingleOrDefault(a => a is DirectoryAttribute) as DirectoryAttribute;
			
			var clusterAttri = attributes.SingleOrDefault(a => a is ClusterAttribute) as ClusterAttribute;
			
			var loadBalanceAttri = attributes.SingleOrDefault(a => a is LoadBalanceAttribute) as LoadBalanceAttribute;
			
			if (directoryAttri == null) {
				throw new PathNotFoundException("directory attri error");
			}
			
			IList<Node> nodes = DirectoryFactory.GetDirectory(directoryAttri.Directory).GetNodes();
			
			if (nodes.Count == 0) {
				throw new NodeNoFoundException("node length is 0");
			}
			
			ICluster cluster = GetCluster(clusterAttri);
			ILoadBalance loadBalance = GetLoadBalance(loadBalanceAttri);
			
			return cluster.Invoke<T>(nodes, directoryAttri.Path, loadBalance, invoker);
		}
		static ILoadBalance GetLoadBalance(LoadBalanceAttribute attri)
		{
			ILoadBalance loadBalance = null;
			if (attri == null) {
				loadBalance = LoadBalanceFactory.GetLoadBalance(RandomLoadBalance.NAME);
			} else {
				loadBalance = LoadBalanceFactory.GetLoadBalance(attri.Name);
			}
			return loadBalance;
		}
		
		static ICluster GetCluster(ClusterAttribute attri)
		{
			ICluster cluster = null;
			if (attri == null) {
				cluster = ClusterFactory.GetCluser(FailoverCluster.NAME);
			} else {
				cluster = ClusterFactory.GetCluser(attri.Name);
			}
			return cluster;
		}
	}
}


















