from scipy import stats
import numpy as np
import pandas as pd
import math
import statsmodels
import matplotlib.pyplot as plt
import glob
import os
import pathlib


df = pd.read_csv("CombinedDataframe.csv")
qdf = pd.read_csv("qdf.csv")
gInfo = pd.read_csv("GInfo.csv")
maxID = df.loc[df['ID'].idxmax()]
print(maxID[1])

modelNames = ["IK mannequin", "mannequinDoggi", "IK dragon ARM"]

testTime = []
trialTime = []
modelTime = []
timeFelt = []
MentalDemandingList = []
physicalDemandingList = []
excitedList = []
controlList = []
hurriedList = []
successfulList = []
hardWorkList = []
insecureList = []
easeList = []

i = 0
'''
# fix 6 og 7 tid
for i in range(maxID[1]):
    if i == 7:
        continue

    print(i+1)
    chunkdf = df[df['ID'] == i+1]
    testTime.append(chunkdf.sum()['Frame Time'])

    individualTrials = []
    for j in range(3):
        print(j+1)
        tempdf = chunkdf[chunkdf['Trail ID'] == j+1]
        individualTrials.append(tempdf.sum()['Frame Time'])
    trialTime.append(individualTrials)

    individualModels = []
    for name in modelNames:
        print(name)
        tempdf = chunkdf[chunkdf['Model Name'] == name]
        individualModels.append(tempdf.sum()['Frame Time'])
    modelTime.append(individualModels)

    timeFeltCols = [col for col in chunkdf.columns if "How many minutes do you feel have passed for this task?" in col]
    timeFeltTrail = []
    for j in timeFeltCols:
        print(j)
        tempdf = chunkdf[j]
        timeFeltTrail.append(tempdf.iloc[0])
    timeFelt.append(timeFeltTrail)

sortedFelt = []
qOrder = gInfo['Questionnaire Order'].values
for index, row in enumerate(timeFelt):
    tempList = []
    if qOrder[index] == 'ABC':
        tempList.extend([row[0] * 60, row[1] * 60, row[2] * 60])

    if qOrder[index] == 'BCA':
        tempList.extend([row[2] * 60, row[0] * 60, row[1] * 60])

    if qOrder[index] == 'CAB':
        tempList.extend([row[1] * 60, row[0] * 60, row[2] * 60])
    sortedFelt.append(tempList)

print(modelTime)
print(timeFelt)
print(sortedFelt)

manData = [[],[]]
dogData = [[],[]]
dragonData = [[],[]]
for index, row in enumerate(sortedFelt):
    manData[0].append(row[0])
    manData[1].append(modelTime[index][0])
    dogData[0].append(row[1])
    dogData[1].append(modelTime[index][1])
    dragonData[0].append(row[2])
    dragonData[1].append(modelTime[index][2])

res = stats.ttest_rel(manData[0], manData[1])
print(res)
res = stats.ttest_rel(dogData[0], dogData[1])
print(res)
res = stats.ttest_rel(dragonData[0], dragonData[1])
print(res)'''

NasaScew = ["negativ",
            "negativ",
            "negativ",
            "positiv",
            "negativ",
            "negativ"]

samScew = ["positiv",
           "positiv",
           "positiv"]

susScew = ["positiv",
           "negativ",
           "positiv",
           "negativ",
           "positiv",
           "negativ",
           "positiv",
           "negativ",
           "positiv",
           "negativ"]

scewMapping = [NasaScew, NasaScew, NasaScew, samScew, samScew, samScew, susScew]

mappedCollection = []
ABCList = []
BCAList = []
CABList = []
def mapLikert(row):
    N = 2
    # remove columns until Participant no.
    newRow = row.iloc[N:]
    firstNasa = newRow.iloc[:len(NasaScew)]
    secondNasa = newRow.iloc[len(NasaScew) + len(samScew): (len(NasaScew) + len(samScew)) + len(NasaScew)]
    thirdNasa = newRow.iloc[(len(NasaScew) + len(samScew)) * 2: ((len(NasaScew) + len(samScew)) * 2) + len(NasaScew)]
    firstSam = newRow.iloc[len(NasaScew): len(NasaScew) + len(samScew)]
    secondSam = newRow.iloc[len(NasaScew) * 2 + len(samScew): (len(NasaScew) * 2 + len(samScew)) + len(samScew)]
    thirdSam = newRow.iloc[(len(NasaScew) * 3) + len(samScew): len(NasaScew) * 3 + len(samScew) + len(samScew)]
    susRow = newRow.tail(len(susScew))

    rowList = [firstNasa, secondNasa, thirdNasa, firstSam, secondSam, thirdSam, susRow]

    mappedList = []
    # if negative subtract 5 if positive subtract 1 and find the absolute value
    for index1, value1 in enumerate(rowList):
        mappedValues = []
        mulNumber = 0
        for index2, value2 in enumerate(value1):
            newValue = 0
            if len(scewMapping[index1]) == 6:
                mulNumber = 2.777
                if scewMapping[index1][index2] == "negativ":
                    newValue = rowList[index1][index2] - 7
                else:
                    newValue = rowList[index1][index2] - 1

            if len(scewMapping[index1]) == 3:
                mulNumber = 5.555
                if scewMapping[index1][index2] == "negativ":
                    newValue = rowList[index1][index2] - 7
                else:
                    newValue = rowList[index1][index2] - 1

            if len(scewMapping[index1]) == 10:
                mulNumber = 2.5
                if scewMapping[index1][index2] == "negativ":
                    newValue = rowList[index1][index2] - 5
                else:
                    newValue = rowList[index1][index2] - 1
            mappedValues.append(abs(newValue))
        mappedList.append(round(sum(mappedValues) * mulNumber, 2))
    mappedCollection.append(mappedList)

def Average(lst):
    return sum(lst) / len(lst)

def calculateMean(data):
    averageData = []
    tData = np.asarray(data).T.tolist()
    for list in tData:
        avg = Average(list)
        averageData.append(round(avg, 2))
    return averageData

def calculateStd(data):
    tempList = []
    tData = np.asarray(data).T.tolist()
    for list in tData:
        tempList.append(round(math.sqrt(Average(list)), 2))
    return tempList

for index, row in qdf.iterrows():
    mapLikert(row)

averageData = calculateMean(mappedCollection)
stdData = calculateStd(mappedCollection)

print("avg data: " + str(averageData))
print("std data: " + str(stdData))


Questionnaires = ['First Nasa', 'Second Nasa', 'Third Nasa', 'First Sam', 'Second Sam', 'Third Sam', 'Sus']
x_pos = np.arange(len(Questionnaires))

fig, ax = plt.subplots()
ax.bar(x_pos, averageData, yerr=stdData, align='center', alpha=0.5, ecolor='black', capsize=10)
ax.set_ylabel('Average questionnaire answer after mapping')
ax.set_xticks(x_pos)
ax.set_xticklabels(Questionnaires)
ax.set_title('Different questionnaires with their mean and standard deviation')
ax.yaxis.grid(True)
plt.show()



qOrder = gInfo['Questionnaire Order'].values

def split(a, n):
    k, m = divmod(len(a), n)
    return (a[i*k+min(i, m):(i+1)*k+min(i+1, m)] for i in range(n))

sortedModelList = []


for index, order in enumerate(qOrder):
    tempgInfo = qdf.iloc[index]
    participantNumber = tempgInfo['Participant no.']

    tempList = []
    susList = tempgInfo.iloc[-10:].values
    tempgInfo = tempgInfo.iloc[:-10]
    tempgInfo = tempgInfo.drop(['Unnamed: 0', 'Participant no.', 'How many minutes do you feel have passed for this task?'
                                , 'How many minutes do you feel have passed for this task?.1', 'How many minutes do you feel have passed for this task?.2']).values

    splitList = list(split(tempgInfo, 3))

    if order == 'ABC':
        tempList.extend(tempgInfo)
        ABCList.extend([splitList[0], splitList[1], splitList[2], susList])

    if order == 'BCA':
        tempList.extend(splitList[2])
        tempList.extend(splitList[0])
        tempList.extend(splitList[1])
        BCAList.extend([splitList[2], splitList[0], splitList[1], susList])

    if order == 'CAB':
        tempList.extend(splitList[1])
        tempList.extend(splitList[2])
        tempList.extend(splitList[0])
        CABList.extend([splitList[1], splitList[2], splitList[0], susList])

    tempList.extend(susList)
    sortedModelList.append(tempList)


sortedMappedList = []

for row in sortedModelList:
    firstNasa = row[:len(NasaScew)]
    secondNasa = row[len(NasaScew) + len(samScew): (len(NasaScew) + len(samScew)) + len(NasaScew)]
    thirdNasa = row[(len(NasaScew) + len(samScew)) * 2: ((len(NasaScew) + len(samScew)) * 2) + len(NasaScew)]
    firstSam = row[len(NasaScew): len(NasaScew) + len(samScew)]
    secondSam = row[len(NasaScew) * 2 + len(samScew): (len(NasaScew) * 2 + len(samScew)) + len(samScew)]
    thirdSam = row[(len(NasaScew) * 3) + len(samScew) * 2: len(NasaScew) * 3 + len(samScew) * 3]
    susRow = row[-len(susScew):]

    rowList = [firstNasa, secondNasa, thirdNasa, firstSam, secondSam, thirdSam, susRow]

    mappedList = []
    # if negative subtract 5 if positive subtract 1 and find the absolute value
    for index1, value1 in enumerate(rowList):
        mappedValues = []
        mulNumber = 0
        for index2, value2 in enumerate(value1):
            newValue = 0
            if len(scewMapping[index1]) == 6:
                mulNumber = 2.777
                if scewMapping[index1][index2] == "negativ":
                    newValue = rowList[index1][index2] - 7
                else:
                    newValue = rowList[index1][index2] - 1

            if len(scewMapping[index1]) == 3:
                mulNumber = 5.555
                if scewMapping[index1][index2] == "negativ":
                    newValue = rowList[index1][index2] - 7
                else:
                    newValue = rowList[index1][index2] - 1

            if len(scewMapping[index1]) == 10:
                mulNumber = 2.5
                if scewMapping[index1][index2] == "negativ":
                    newValue = rowList[index1][index2] - 5
                else:
                    newValue = rowList[index1][index2] - 1
            mappedValues.append(abs(newValue))
        mappedList.append(round(sum(mappedValues) * mulNumber, 2))
    sortedMappedList.append(mappedList)


sAverageData = calculateMean(sortedMappedList)
sStdData = calculateStd(sortedMappedList)

print("avg data: " + str(sAverageData))
print("std data: " + str(sStdData))


Questionnaires = ['Human NASA', 'Dog NASA', 'Dragon NASA', 'Human SAM', 'Dog SAM', 'Dragon SAM', 'SUS']
x_pos = np.arange(len(Questionnaires))
tick = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100]

fig, ax = plt.subplots()
ax.bar(x_pos, sAverageData, yerr=sStdData, align='center', alpha=0.5, ecolor='black', capsize=10)
ax.set_ylabel('Average questionnaire scores')
plt.ylim(0, 100)
for i, value in enumerate(sAverageData):
    ax.text(i, value + 14, "M = " + str(value), color='black', horizontalalignment='center')
    ax.text(i, value + 10, "S = " + str(sStdData[i]), color='black', horizontalalignment='center')
ax.set_xticks(x_pos)
ax.set_xticklabels(Questionnaires)
ax.set_yticks(tick) # Grid
ax.set_title('Different questionnaires with their mean and standard deviation')
ax.yaxis.grid(True)
plt.show()



modelQuestionnaire = [ABCList, BCAList, CABList]
avgdata = []
stddata = []
fig, ax = plt.subplots()
for index, list in enumerate(modelQuestionnaire):
    sortedMappedList = []
    conList = np.concatenate((list[0], list[1], list[2], list[3]), axis=None)
    for row in conList:
        firstNasa = conList[:len(NasaScew)]
        secondNasa = conList[len(NasaScew) + len(samScew): (len(NasaScew) + len(samScew)) + len(NasaScew)]
        thirdNasa = conList[(len(NasaScew) + len(samScew)) * 2: ((len(NasaScew) + len(samScew)) * 2) + len(NasaScew)]
        firstSam = conList[len(NasaScew): len(NasaScew) + len(samScew)]
        secondSam = conList[len(NasaScew) * 2 + len(samScew): (len(NasaScew) * 2 + len(samScew)) + len(samScew)]
        thirdSam = conList[(len(NasaScew) * 3) + len(samScew) * 2: len(NasaScew) * 3 + len(samScew) * 3]
        susRow = conList[-len(susScew):]

        rowList = [firstNasa, secondNasa, thirdNasa, firstSam, secondSam, thirdSam, susRow]

        mappedList = []
        # if negative subtract 5 if positive subtract 1 and find the absolute value
        for index1, value1 in enumerate(rowList):
            mappedValues = []
            mulNumber = 0
            for index2, value2 in enumerate(value1):
                newValue = 0
                if len(scewMapping[index1]) == 6:
                    mulNumber = 2.777
                    if scewMapping[index1][index2] == "negativ":
                        newValue = rowList[index1][index2] - 7
                    else:
                        newValue = rowList[index1][index2] - 1

                if len(scewMapping[index1]) == 3:
                    mulNumber = 5.555
                    if scewMapping[index1][index2] == "negativ":
                        newValue = rowList[index1][index2] - 7
                    else:
                        newValue = rowList[index1][index2] - 1

                if len(scewMapping[index1]) == 10:
                    mulNumber = 2.5
                    if scewMapping[index1][index2] == "negativ":
                        newValue = rowList[index1][index2] - 5
                    else:
                        newValue = rowList[index1][index2] - 1
                mappedValues.append(abs(newValue))
            mappedList.append(round(sum(mappedValues) * mulNumber, 2))
        sortedMappedList.append(mappedList)

    sAverageData = calculateMean(sortedMappedList)
    sStdData = calculateStd(sortedMappedList)

    print("avg data: " + str(sAverageData))
    print("std data: " + str(sStdData))

    avgdata.append(sAverageData)
    stddata.append(sStdData)

    Questionnaires = ['Human NASA', 'Dog NASA', 'Dragon NASA', 'Human SAM', 'Dog SAM', 'Dragon SAM', 'SUS']
    x_pos = np.arange(len(Questionnaires))

    ax.set_ylabel('Average questionnaire scores')
    plt.ylim(0, 100)
    for i, value in enumerate(avgdata[index]):
        ax.text(i * .25, value + 3, str(value), color='black', fontweight='bold')
    ax.set_xticks(x_pos)
    ax.set_xticklabels(Questionnaires)
    ax.set_title('Questionnaires with mean and standard deviation')
    ax.bar(x_pos + 0.25 * index, avgdata[index], yerr=stdData, ecolor='black', width=0.25)
    ax.yaxis.grid(True)
ax.legend(labels=['ABC', 'BCA', 'CAB'])
plt.show()