export const UpdateFactory = {
    createPitUpdate: (pitInUpdate, pitOutUpdate) => {
        return {
            previousDriver: pitInUpdate.shortName,
            newDriver: pitOutUpdate.shortName,
            pitLaneEntryTime: pitInUpdate.sessionTimeLeft,
            pitLaneExitTime: pitOutUpdate.sessionTimeLeft,
            pitStopTime: pitInUpdate.sessionTimeLeft - pitOutUpdate.sessionTimeLeft,
        };
    },
};