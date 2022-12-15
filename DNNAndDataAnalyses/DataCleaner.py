import numpy as np
import csv
import pandas as pd
import numpy as np
from numpy import genfromtxt
my_data = genfromtxt('CleanData.csv', delimiter=',')

print(my_data)

coordinates = []

def NormalizeData(data):
    return (data - np.min(data)) / (np.max(data) - np.min(data))

combined = []
for row in my_data:
    NormalizeData(row)
    coordinates.append(row.tolist())

print(coordinates)

# using the savetxt
# from the numpy module
np.savetxt("NormData.csv",
           np.asarray(coordinates),
           delimiter =", ",
           fmt ='% s')