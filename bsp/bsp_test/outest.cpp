// out test for read file for seam find. 
// C++ version.
#include <iostream>
#include <fstream>
#include <cstdlib>

int main()
{
    using namespace std;

    ifstream inFile;
    inFile.open("gdata.dat");

    if (!inFile.is_open())
    {
        cout << "Can't open the file" << endl;
        exit(EXIT_FAILURE);
    }

    char ch;
    float pixel[300];
    for (int i = 0; i < 10; i++)
    {
        inFile >> pixel[i];
        ch = inFile.get();  // a period.
        cout << pixel[i] << endl;
    }

    inFile.close();
    return 0;
}
