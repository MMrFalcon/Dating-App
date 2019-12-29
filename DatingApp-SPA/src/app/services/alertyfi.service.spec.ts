/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { AlertyfiService } from './alertyfi.service';

describe('Service: Alertyfi', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AlertyfiService]
    });
  });

  it('should ...', inject([AlertyfiService], (service: AlertyfiService) => {
    expect(service).toBeTruthy();
  }));
});
