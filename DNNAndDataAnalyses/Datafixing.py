import pandas as pd
import os
import pathlib

path = "C:/Users/mikael/Documents/p7Kanditat/OculusQuest/Assets/TestData"

arr = os.listdir(path)
li = []
indexAmount = 0
for filename in arr:
    fileEx = pathlib.Path(filename).suffix
    if fileEx != ".meta":
        indexAmount += 1
        df = pd.read_csv(path + "/" + filename, index_col=None, header=0, sep=';')
        if 7 in df['ID'].values:
            df = df['ID'].replace(6, 7)
        li.append(df)

fulldf = pd.concat(li, ignore_index=True)



taskdf = fulldf[fulldf['ID'] != -1]
taskdf = taskdf[taskdf['Between tasks'] == 0]
taskdf = taskdf[taskdf['Model Name'] != "mannequinbold"]
taskdf.loc[taskdf['ID'] == 5, "Model order"] = "cab"


maxID = taskdf.loc[taskdf['ID'].idxmax()]

GIdf = pd.read_csv("GInfo.csv")

for name, values in GIdf.iteritems():
    taskdf[name] = GIdf[name]
    for i in range(maxID[0]):
        taskdf.loc[taskdf['ID'] == i + 1, name] = GIdf[name][i]
print(taskdf)

abcdf = pd.read_csv("Questionnaire ABC.csv")
bcadf = pd.read_csv("Questionnaire BCA.csv")
cabdf = pd.read_csv("Questionnaire CAB.csv")

qdf = pd.concat([abcdf, bcadf, cabdf], ignore_index=True)
qdf = qdf.drop(['Tidsstempel'], axis=1)
sqdf = qdf.sort_values(by=['Participant no.'])
sqdf.to_csv('qdf.csv')
print(sqdf)

for name, values in sqdf.iteritems():
    taskdf[name] = sqdf[name]
    for i in range(maxID[0]):
        taskdf.loc[taskdf['ID'] == i + 1, name] = sqdf[name].iloc[[i]].item()


#taskdf = taskdf.drop(['Participant', 'Questionnaire Order', 'Participant no.', 'Tidsstempel'], axis=1)
#taskdf = taskdf.drop(['Questionnaire Order', 'Tidsstempel'], axis=1)
print(taskdf)
taskdf.to_csv('CombinedDataframe.csv')


