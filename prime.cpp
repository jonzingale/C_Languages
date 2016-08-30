/*--------------------------------------------------------*/
/*                                                        */
/* To compile:  g++ -o prime prime.cpp                    */
/*                                                        */
/*--------------------------------------------------------*/
/* prime.cpp  Mark Brettin  2/23/00                       */
/*                                                        */
/* For any integer a and any prime number p,              */
/* if p divides a, then it does not divide (a+1).         */
/*                                                        */
/* A positive integer p is a prime number if it is not    */
/* evenly divisible by any other integers other than 1    */
/* and itself.                                            */
/*--------------------------------------------------------*/

// #include <stdio>
// #include <stdlib>
#include <iostream>
#include <string>
// #include <iomanip>
using namespace std;
#define MAX_PRIMES 10000

void displayBits(long value);


int main(void) {
  unsigned long possibly_prime;
  unsigned long primes_array[MAX_PRIMES];  // this should be a linked list

  unsigned long i = 2;
  int is_prime, prev_index, next_index = 2; 

  primes_array[0] = 2;    // first prime number is defined
  primes_array[1] = 3;    // second prime number is defined

  for(possibly_prime = 5; possibly_prime <= MAX_PRIMES; possibly_prime += 2) {
      is_prime = 1;
 
      for(prev_index = 1; 
            (is_prime && (possibly_prime/primes_array[prev_index]
                >= primes_array[prev_index]));
            ++prev_index)
      {
        if(possibly_prime % primes_array[prev_index] == 0)
          is_prime = 0;
      }

      if(is_prime) { 
          primes_array[next_index] = possibly_prime;
          printf("%d\t%x\t", primes_array[next_index], primes_array[next_index]);
          displayBits(primes_array[next_index]);
          printf("\n");
          next_index++;
          i++;
      }
      if(i >= MAX_PRIMES) { break; }
  }
  printf("\n%d prime numbers generated\n", i);

  return(0);
}


void displayBits(long value)
{
  long one = 1;
  long displayMask = one << ((sizeof(long) *8) -1);

  //cout << setw(5) << value << " : ";
  for(int i=1; i<=(sizeof(long) *8); i++)
  {
    cout << (value & displayMask ? '1' : '0');
    value <<= 1;

    if(i % 8 == 0)
      cout << " ";
  }
}


