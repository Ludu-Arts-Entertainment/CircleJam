using System;

public partial class Events
{
    public struct AskDataSyncSourceEvent : IEvent
    {
        public Action<DataSourceType> OnDataSelected;
        public readonly BasicProgressSummaryCardModel RemoteBasicProgressSummaryCardData;
        public readonly BasicProgressSummaryCardModel LocalBasicProgressSummaryCardData;
        
        public AskDataSyncSourceEvent(BasicProgressSummaryCardModel remoteBasicProgressSummaryCardData, BasicProgressSummaryCardModel localBasicProgressSummaryCardData, Action<DataSourceType> onDataSelected = null)
        {
            OnDataSelected = onDataSelected;
            RemoteBasicProgressSummaryCardData = remoteBasicProgressSummaryCardData;
            LocalBasicProgressSummaryCardData = localBasicProgressSummaryCardData;
        }
    }
}
