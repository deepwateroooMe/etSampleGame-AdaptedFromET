//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月23日 15:44:40
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using ETModel.BBValues;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class NP_RuntimeTreeRepositoryAwakeSystem: AwakeSystem<NP_TreeDataRepository>
    {
        public override void Awake(NP_TreeDataRepository self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 行为树数据仓库组件
    /// </summary>
    public class NP_TreeDataRepository: Component
    {
#if SERVER
        public const string NPDataPath = "../Config/SkillConfigs/Server/";
#else
        public const string NPDataPath = "../Config/SkillConfigs/Client/";
#endif

        /// <summary>
        /// 运行时的行为树仓库，注意，一定不能对这些数据做修改
        /// </summary>
        private Dictionary<long, NP_DataSupportor> m_NpRuntimeTreesDatas = new Dictionary<long, NP_DataSupportor>();

        public void Awake()
        {
#if SERVER
            DirectoryInfo directory = new DirectoryInfo(NPDataPath);
            FileInfo[] fileInfos = directory.GetFiles();

            foreach (var fileInfo in fileInfos)
            {
                byte[] mfile = File.ReadAllBytes(fileInfo.FullName);

                if (mfile.Length == 0) Log.Info("没有读取到文件");

                try
                {
                    NP_DataSupportor MnNpDataSupportor = BsonSerializer.Deserialize<NP_DataSupportor>(mfile);

                    Log.Info($"反序列化行为树：id：{MnNpDataSupportor.NpDataSupportorBase.NPBehaveTreeDataId} {fileInfo.FullName}完成");

                    m_NpRuntimeTreesDatas.Add(MnNpDataSupportor.NpDataSupportorBase.NPBehaveTreeDataId, MnNpDataSupportor);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
#else

            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle("skillconfigs.unity3d");
            GameObject skillConfigs = (GameObject) resourcesComponent.GetAsset("skillconfigs.unity3d", "SkillConfigs");
            foreach (var referenceCollectorData in skillConfigs.GetComponent<ReferenceCollector>().data)
            {
                TextAsset textAsset = skillConfigs.GetTargetObjectFromRC<TextAsset>(referenceCollectorData.key);

                if (textAsset.bytes.Length == 0) Log.Info("没有读取到文件");

                try
                {
                    NP_DataSupportor MnNpDataSupportor = BsonSerializer.Deserialize<NP_DataSupportor>(textAsset.bytes);

                    Log.Info($"反序列化行为树:{referenceCollectorData.key}完成");

                    this.m_NpRuntimeTreesDatas.Add(MnNpDataSupportor.NpDataSupportorBase.RootId, MnNpDataSupportor);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    throw;
                }
            }

#endif
        }
        
        /// <summary>
        /// 获取一棵树的所有数据（默认形式）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NP_DataSupportor GetNP_TreeData(long id)
        {
            if (this.m_NpRuntimeTreesDatas.ContainsKey(id))
            {
                return this.m_NpRuntimeTreesDatas[id];
            }

            Log.Error($"请求的行为树id不存在，id为{id}");
            return null;
        }

        /// <summary>
        /// 获取一棵树的所有数据（仅深拷贝黑板数据内容，而忽略例如BuffNodeDataDic的数据内容）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NP_DataSupportor GetNP_TreeData_DeepCopyBBValuesOnly(long id)
        {
            NP_DataSupportor result = new NP_DataSupportor();
            if (this.m_NpRuntimeTreesDatas.ContainsKey(id))
            {
                result.BuffNodeDataDic = this.m_NpRuntimeTreesDatas[id].BuffNodeDataDic;
                result.NpDataSupportorBase = this.m_NpRuntimeTreesDatas[id].NpDataSupportorBase.DeepCopy();
                return result;
            }

            Log.Error($"请求的行为树id不存在，id为{id}");
            return null;
        }
        
        /// <summary>
        /// 获取一棵树的所有数据（通过深拷贝形式）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NP_DataSupportor GetNP_TreeData_DeepCopy(long id)
        {
            if (this.m_NpRuntimeTreesDatas.ContainsKey(id))
            {
                return this.m_NpRuntimeTreesDatas[id].DeepCopy();
            }

            Log.Error($"请求的行为树id不存在，id为{id}");
            return null;
        }
    }
}