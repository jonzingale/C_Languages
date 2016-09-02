#include <iostream>
#include <string>
using namespace std;

bool is_prime(unsigned long long n) {
  unsigned long long i;
  bool retval = true;

  for (unsigned long long i = 2; i * i <= n; ++i) {
    if (n % i == 0) {
      cout << n << " is divisible by " << i << endl;
      retval = false;
    }
  }
  return retval;
} 


int main(int argc, char *argv[]) {
  unsigned long long n;

  if ( argc < 2 ) // argc should be 2 for correct execution
    // We print argv[0] assuming it is the program name
    cout<<"usage: "<< argv[0] <<" <prime_to_test>\n";
  else {
    n = stoull(argv[1]);
    if (is_prime(n)) {
      cout << n << " is prime!" << endl;
    } else {
      cout << n << " is NOT prime" << endl;
    }
  }
  return 0;
}
