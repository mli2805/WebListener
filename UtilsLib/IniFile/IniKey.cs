namespace UtilsLib
{
    public enum IniKey
    {
        // Server
        IsWithoutMapMode,
        RtuGuid,
        HasReserveAddress,
        ServerTitle,
        Ip,
        Host,
        IsAddressIp,
     //   TcpPort,
        ClientOrdinal, // will be added to to the TCP port number 11843; for Client is 0, but under SuperClient would be different

        // General
        Version,
        Culture,
        LogFileSizeLimitKb,
        LogLevel,

        // General - Client
        ClientPollingRateMs,
        FailedPollsLimit,
        Ip4Default,
        // General - RTU
        RtuHeartbeatRate,
        // General - DataCenter
        CheckHeartbeatEvery,
        ClientConnectionsPermittedGap,
        RtuHeartbeatPermittedGap,

        EventSourcingPortion,

        // RtuManager
        OtdrIp,
        OtdrPort,
        OtauIp,
        OtauPort,
        PreviousOwnPortCount,
        ShouldLogHeartbeatProblems,

        // Migrator
        OldFibertestServerIp,  
        ShouldTransferMeasurements,
        Kadastr,

      
        // Charon
        ConnectionTimeout,
        ReadTimeout,
        WriteTimeout,
        ComPort,
        ComSpeed,
        PauseAfterReset,
        PauseBetweenCommandsMs,

        // Monitoring
        IsMonitoringOn,
        PreciseMakeTimespan,
        PreciseSaveTimespan,
        FastSaveTimespan,
        LastMeasurementTimestamp,

        // Recovering
        MikrotikRebootTimeout,
        MikrotikRebootAttemptsBeforeNotification,
        RecoveryStep,
        RebootSystemEnabled,
        RebootSystemDelay,


        // Watchdog
        RtuServiceName,
        LastRestartTime,
        MaxGapBetweenMeasurements,

        // NetTcpBinding
        OpenTimeoutMs,
        ReceiveTimeout,
        SendTimeout,
        PingTimeoutMs,

        //MySql
        MySqlTcpPort,
        MySqlDbSchemePostfix,
        ResetDb,
        FreeSpaceThresholdGb,
        IsOptimizationCouldBeDoneUpToToday,
        SnapshotUptoLimitInDays,

        //Broadcast
        GsmModemComPort,
        EventLifetimeLimit,
        TestNumberToSms,
        TestSmsContent,
        MsmqTestRtuId,
        MsmqTestTraceId,

        // Smtp
        SmtpHost,
        SmtpPort,
        MailFrom,
        MailFromPassword,
        SmtpTimeoutMs,

        // Snmp,
        IsSnmpOn,
        SnmpTrapVersion,
        SnmpReceiverIp,
        SnmpReceiverPort,
        SnmpAgentIp,
        SnmpCommunity,
        EnterpriseOid,
        SnmpEncoding,

        // Map
        Zoom,
        CenterLatitude,
        CenterLongitude,
        MaxZoom,
        SaveMaxZoomNoMoreThan,
        GMapProvider,

        // Miscellaneous
        PathToSor,
        MaxCableReserve,
        GpsInputMode,
        GraphVisibilityLevel,
        IsGraphVisibleOnStart,
        PathToClient,

        // LoadTesting
        Multiplier,
        Pause,

        // WebApi
        BindingProtocol,

        // Sqlite
        DbPath,
    }
}